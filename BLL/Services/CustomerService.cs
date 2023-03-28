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
    public class CustomerService : ICustomerService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IRedisService _redis; 
        private readonly IKafkaSender _kafkaSender;
        private readonly ILogger<CustomerService> _logger;
        private string topic = "";

        public CustomerService(
            IUnitOfWork unitOfWork,
             IConfiguration config,
            IRedisService redis, 
            IKafkaSender kafkaSender,
            ILogger<CustomerService> logger)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _redis = redis;
            _kafkaSender = kafkaSender;
            _logger = logger;
            topic = _config.GetValue<string>("Topic:VerifyConsumer");
        }

        public async Task<List<Customer>> GetAllCustomerAsync()
        {
            return await _unitOfWork.CustomerRepository.GetAll()
                .ToListAsync();
        }

        public async Task<Customer> GetCustomerByIdAsync(Guid CustomerId)
        {
            _logger.LogInformation("Getting Customer By Id");
            Customer Customer = await _redis.GetAsync<Customer>($"{PrefixRedisKey.CustomerKey}:{CustomerId}");

            if (Customer == null)
            {
                Customer = await _unitOfWork.CustomerRepository.GetAll()
                    .Where(x => x.CustomerId == CustomerId)
                    .FirstOrDefaultAsync();

                TimeSpan expirition = new TimeSpan(1, 0, 00);
                await _redis.SaveAsync($"{PrefixRedisKey.CustomerKey}:{CustomerId}", Customer, expirition);
            }

            return Customer;
        }


        public async Task VerifyingCustomer(Sales data)
        {
            _logger.LogInformation($"Verifying Customer Status with Customer ID {data.CustomerId} for Sales/Order ID {data.SalesId}");
            Customer customerData = await GetCustomerByIdAsync(data.CustomerId);

            string statusCustomer = CustomerStatus.NotFound;

            if (customerData != null)
            {
                statusCustomer = customerData.CustomerIsActive ? CustomerStatus.Active : CustomerStatus.InActive;
            }

            var verifyingCustomerDTO = new VerifyingCustomerDTO()
            {
                SalesId = data.SalesId,
                CustomerId = data.CustomerId,
                CustomerStatus = statusCustomer
            };

            _logger.LogInformation($"Send data with Sales/Order ID {data.SalesId} to Kafka with Topic : VerifyCustomer");
            await _kafkaSender.SendAsync(topic, verifyingCustomerDTO);
        }

        /*
        
        public async Task CreateCustomerAsync(Customer data)
        {
            await _unitOfWork.CustomerRepository.AddAsync(data);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateCustomerAsync(Customer data)
        {
            bool isExist = _unitOfWork.CustomerRepository.IsExist(x => x.CustomerId == data.CustomerId);

            if (!isExist)
            {
                throw new Exception($"Customer with id {data.CustomerId} not exist");

            }
            _unitOfWork.CustomerRepository.Edit(data);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteCustomerAsync(Guid CustomerId)
        {
            var isExist = _unitOfWork.CustomerRepository.IsExist(x => x.CustomerId == CustomerId);
            if (!isExist)
            {
                throw new Exception($"Customer with id {CustomerId} not exist");
            }

            _unitOfWork.CustomerRepository.Delete(x => x.CustomerId == CustomerId);
            await _unitOfWork.SaveAsync();
        }
        */
    }

    
}
