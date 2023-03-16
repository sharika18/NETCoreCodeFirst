using BLL.Cache;
using BLL.DTO;
using BLL.Interfaces;
using BLL.Kafka;
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
        private string topicVerifyConsumer = "";

        public SalesService(
            IUnitOfWork unitOfWork, 
            IConfiguration config, 
            IKafkaSender kafkaSender, 
            IRedisService redis,
            ILogger<SalesService> logger)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _kafkaSender = kafkaSender;
            _redis = redis;
            _logger = logger;
            topicVerifyConsumer = _config.GetValue<string>("Topic:OrderCreated");
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
            _logger.LogInformation("Getting All Data Sales");
            var result = await _unitOfWork.SalesRepository.GetAll()
                .Where(x => x.SalesId == SalesId)
                .FirstOrDefaultAsync();
            return result;
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

            await _kafkaSender.SendAsync(topicVerifyConsumer, data);
        }

        public async Task UpdateSalesAsync(Sales data)
        {
            bool isExist = _unitOfWork.SalesRepository.IsExist(x => x.SalesId == data.SalesId);

            if (!isExist)
            {
                throw new Exception($"Sales with id {data.SalesId} not exist");

            }
            _unitOfWork.SalesRepository.Edit(data);
            await _unitOfWork.SaveAsync();
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


    }

    public static class SalesStatus
    {
        public const string Verifying = "Verifying";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
    }
}
