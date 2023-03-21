using BLL.Cache;
using BLL.DTO;
using BLL.Interfaces;
using BLL.Kafka;
using DAL.Interfaces;
using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
namespace BLL.Services
{
    public class SalesService : ISalesService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IKafkaSender _kafkaSender;
        private IRedisService _redis;
        private readonly ILogger<SalesService> _logger;
        private string topic = "";

        public SalesService
            (
                IUnitOfWork unitOfWork, 
                IConfiguration config, 
                IKafkaSender kafkaSender, 
                IRedisService redis,
                ILogger<SalesService> logger
            )
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _kafkaSender = kafkaSender;
            _redis = redis;
            _logger = logger;
            topic = _config.GetValue<string>("Topic:OrderCreated");
        }

        public async Task<List<Sales>> GetAllSalesAsync()
        {
            return await _unitOfWork.SalesRepository.GetAll()
                .Include(a => a.Product)
                .Include(a => a.Customer)
                .Include(a => a.Territories)
                .ToListAsync();
        }

        public async Task<Sales> GetSalesByIdAsync(Guid SalesId)
        {
            _logger.LogInformation("Getting Sales By Id");
            Sales Sales = await _redis.GetAsync<Sales>($"{PrefixRedisKey.SalesKey}:{SalesId}");

            if (Sales == null)
            {
                Sales = await _unitOfWork.SalesRepository.GetAll()
                    .Where(x => x.SalesId == SalesId)
                    .Include(a => a.Product)
                    .Include(a => a.Customer)
                    .Include(a => a.Territories)
                    .FirstOrDefaultAsync();

                TimeSpan expirition = new TimeSpan(1, 0, 00);
                await _redis.SaveAsync($"{PrefixRedisKey.SalesKey}:{SalesId}", Sales, expirition);
            }

            return Sales;
        }

        public async Task<bool> IsSalesStatusVerifying(Guid CustomerId)
        {
            bool isVerifying = false;
            var dataSales =  _unitOfWork.SalesRepository.GetAll().Where(x => x.CustomerId == CustomerId).FirstOrDefault();
            if (dataSales != null)
            {
                isVerifying = dataSales.SalesStatus == SalesStatus.Verifying ? true : false;
            }

            return isVerifying;
        }

        public async Task<Sales> AssignPrice(Sales data)
        {
            _logger.LogInformation($"Verifying previous status Sales/Order");
            bool isVerifying = await IsSalesStatusVerifying(data.CustomerId);

            if (isVerifying)
            {
                throw new Exception($"Customer with ID {data.CustomerId} still has unfinished previous order");
            }

            _logger.LogInformation($"Checking product price");
            var dataProduct = await _unitOfWork.ProductRepository.GetByIdAsync(data.ProductId);

            data.UnitPrice = dataProduct.ListPrice;
            data.SalesAmount = data.OrderQuantity * data.UnitPrice;
            data.SalesStatus = SalesStatus.Verifying;

            return data;
        }

        public async Task CreateSalesAsync(Sales data)
        {
            var finalData = await AssignPrice(data);

            _logger.LogInformation($"Saving data to database and redis");
            await _unitOfWork.SalesRepository.AddAsync(finalData);
            await _unitOfWork.SaveAsync();

            TimeSpan expirition = new TimeSpan(2, 0, 00);
            await _redis.SaveAsync($"{PrefixRedisKey.SalesKey}:{finalData.SalesId}", finalData, expirition);
            //var messageSendToKafka = new VerifyingCustomerDTO()
            //{
            //    SalesId = data.SalesId,
            //    CustomerId = data.CustomerId
            //};

            _logger.LogInformation($"Send data with Sales/Order ID {finalData.SalesId} to Kafka with Topic : OrderCreated");
            await _kafkaSender.SendAsync(topic, finalData);
        }

        public async Task UpdateSalesAsync(Sales data)
        {
            _logger.LogInformation($"Check if Sales/Order ID {data.SalesId} is Exist");
            bool isExist = _unitOfWork.SalesRepository.IsExist(x => x.SalesId == data.SalesId);

            if (!isExist)
            {
                throw new Exception($"Sales with id {data.SalesId} not exist");

            }

            try
            {
                var finalData = await AssignPrice(data);

                data.UnitPrice = finalData.UnitPrice;
                data.SalesAmount = finalData.SalesAmount;
                data.SalesStatus = finalData.SalesStatus;
                
                _unitOfWork.SalesRepository.Edit(data);


                _logger.LogInformation($"Saving to database, Delete Previous data from redis, Savinf new data to redis");
                await _unitOfWork.SaveAsync();
                await _redis.DeleteAsync($"{PrefixRedisKey.SalesKey}:{data.SalesId}");

                TimeSpan expirition = new TimeSpan(2, 0, 00);
                await _redis.SaveAsync($"{PrefixRedisKey.SalesKey}:{data.SalesId}", data, expirition);
                //var messageSendToKafka = new VerifyingCustomerDTO()
                //{
                //    SalesId = data.SalesId,
                //    CustomerId = data.CustomerId
                //};

                _logger.LogInformation($"Send data with Sales/Order ID {data.SalesId} to Kafka with Topic : OrderCreated");
                await _kafkaSender.SendAsync(topic, data);
            }
            catch (Exception e)
            {
                
            }
        }


        public async Task ApproveRejectSales(VerifyingCustomerDTO verifyingCustomerdata)
        {
            _logger.LogInformation($"Update Sales/Order Statuswith Customer ID {verifyingCustomerdata.CustomerId} for Sales/Order ID {verifyingCustomerdata.SalesId}");
            Sales data = await _redis.GetAsync<Sales>($"{PrefixRedisKey.SalesKey}:{verifyingCustomerdata.SalesId}");
            if (data == null)
            {
                data = await GetSalesByIdAsync(verifyingCustomerdata.SalesId);
            }

            if (data != null)
            {
                data.SalesStatus =
                verifyingCustomerdata.CustomerStatus == CustomerStatus.Active ?
                SalesStatus.Approved : SalesStatus.Rejected;

                if (data.SalesStatus == SalesStatus.Approved)
                {
                    _logger.LogInformation($"Order Approved");
                }
                else
                {
                    _logger.LogInformation($"Order Rejected either because Customer is InActive or Not Found");
                }

                await UpdateSalesAsync(data);
                await _redis.DeleteAsync($"{PrefixRedisKey.SalesKey}:{verifyingCustomerdata.SalesId}");
            }
        }

        public async Task<List<Territories>> GetAllTerritoriesAsync()
        {
            return await _unitOfWork.TerritoriesRepository.GetAll()
                .ToListAsync();
        }

    }

    
}
