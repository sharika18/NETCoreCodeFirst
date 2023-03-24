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

        private TimeSpan expirition = new TimeSpan(2, 0, 00);

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
                await _redis.SaveAsync($"{PrefixRedisKey.SalesKey}:{SalesId}", Sales, expirition);
            }

            return Sales;
        }

        public async Task IsSalesStatusVerifying(Guid CustomerId)
        {
            _logger.LogInformation($"Verifying previous status Sales/Order");
            bool isVerifying = false;
            var prevSalesStatus =  _unitOfWork.SalesRepository.GetAll()
                                    .Where(x => x.CustomerId == CustomerId)
                                    .OrderByDescending(x => x.OrderDate)
                                    .Select(x => x.SalesStatus)
                                    .FirstOrDefault();
            if (prevSalesStatus != null)
            {
                isVerifying = prevSalesStatus == SalesStatus.Verifying ? true : false;
            }

            if (isVerifying)
            {
                throw new Exception($"Customer with ID {CustomerId} still has unfinished previous order");
            }
        }

        public async Task<Sales> AssignSalesValue(Sales data)
        {
            await IsSalesStatusVerifying(data.CustomerId);

            _logger.LogInformation($"Checking product price");
            var dataProduct = await _unitOfWork.ProductRepository.GetSingleAsync(x => x.ProductId == data.ProductId);
            
            if (dataProduct == null)
            {
                throw new Exception($"Product ID {data.ProductId} not found");
            }

            data.UnitPrice = dataProduct.ListPrice;
            data.SalesAmount = data.OrderQuantity * dataProduct.ListPrice;
            data.SalesStatus = SalesStatus.Verifying;

            return data;
        }

        public async Task CreateSalesAsync(Sales data)
        {
            var assignSalesValue = await AssignSalesValue(data);
            data.UnitPrice = assignSalesValue.UnitPrice;
            data.SalesAmount = assignSalesValue.SalesAmount;
            data.SalesStatus = assignSalesValue.SalesStatus;

            _logger.LogInformation($"Saving data to database and redis");
            await _unitOfWork.SalesRepository.AddAsync(data);
            await _unitOfWork.SaveAsync();

            await _redis.SaveAsync($"{PrefixRedisKey.SalesKey}:{data.SalesId}", data, expirition);
            
            await SendToOrderCreated(data);
        }

        public async Task UpdateSalesAsync(Sales data)
        { 
            var assignSalesValue = await AssignSalesValue(data);
            data.UnitPrice = assignSalesValue.UnitPrice;
            data.SalesAmount = assignSalesValue.SalesAmount;
            data.SalesStatus = assignSalesValue.SalesStatus;
            await UpdateSales(data);
            await SendToOrderCreated(data);
        }

        public async Task UpdateSales(Sales data)
        {
            _logger.LogInformation($"Check if Sales/Order ID {data.SalesId} is Exist");
            bool isExist = _unitOfWork.SalesRepository.IsExist(x => x.SalesId == data.SalesId);

            if (!isExist)
            {
                throw new Exception($"Sales with id {data.SalesId} not exist");

            }

            _unitOfWork.SalesRepository.Edit(data);

            _logger.LogInformation($"Saving to database, Delete Previous data from redis, Savinf new data to redis");
            await _unitOfWork.SaveAsync();

            
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

                await UpdateSales(data);
                await _redis.DeleteAsync($"{PrefixRedisKey.SalesKey}:{data.SalesId}");
                await _redis.SaveAsync($"{PrefixRedisKey.SalesKey}:{data.SalesId}", data, expirition);
            }
        }

        public async Task SendToOrderCreated(Sales data)
        {
            _logger.LogInformation($"Send data with Sales/Order ID {data.SalesId} to Kafka with Topic : OrderCreated");
            await _kafkaSender.SendAsync(topic, data);
        }

        public async Task<List<Territories>> GetAllTerritoriesAsync()
        {
            return await _unitOfWork.TerritoriesRepository.GetAll()
                .ToListAsync();
        }

    }

    
}
