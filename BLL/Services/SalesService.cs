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
                    .FirstOrDefaultAsync();

                TimeSpan expirition = new TimeSpan(1, 0, 00);
                await _redis.SaveAsync($"{PrefixRedisKey.SalesKey}:{SalesId}", Sales, expirition);
            }

            return Sales;
        }

        public async Task CreateSalesAsync(Sales data)
        {
            var dataProduct = await _unitOfWork.ProductRepository.GetByIdAsync(data.ProductId);

            data.UnitPrice = dataProduct.ListPrice;
            data.SalesAmount = data.OrderQuantity * data.UnitPrice;
            data.SalesStatus = SalesStatus.Verifying;

            await _unitOfWork.SalesRepository.AddAsync(data);
            await _unitOfWork.SaveAsync();

            TimeSpan expirition = new TimeSpan(2, 0, 00);
            await _redis.SaveAsync($"{PrefixRedisKey.SalesKey}:{data.SalesId}", data, expirition);
            //var messageSendToKafka = new VerifyingCustomerDTO()
            //{
            //    SalesId = data.SalesId,
            //    CustomerId = data.CustomerId
            //};

            await _kafkaSender.SendAsync(topic, data);
        }

        public async Task<Sales> UpdateSalesAsync(Sales data)
        {
            bool isExist = _unitOfWork.SalesRepository.IsExist(x => x.SalesId == data.SalesId);

            if (!isExist)
            {
                throw new Exception($"Sales with id {data.SalesId} not exist");

            }
            _unitOfWork.SalesRepository.Edit(data);
            await _unitOfWork.SaveAsync();
            return data;
        }

        public async Task DeleteSalesAsync(Guid SalesId)
        {
            var isExist = _unitOfWork.SalesRepository.IsExist(x => x.SalesId == SalesId);
            if (!isExist)
            {
                throw new Exception($"Sales with id {SalesId} not exist");
            }

            _unitOfWork.SalesRepository.Delete(x => x.SalesId == SalesId);
            await _unitOfWork.SaveAsync();
        }

        public async Task ApproveRejectSales(VerifyingCustomerDTO verifyingCustomerdata)
        {
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

                await UpdateSalesAsync(data);
                await _redis.DeleteAsync($"{PrefixRedisKey.SalesKey}:{verifyingCustomerdata.SalesId}");
            }
        }

    }

    
}
