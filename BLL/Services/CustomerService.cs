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
    public class CustomerService : ICustomerService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IKafkaSender _kafkaSender;
        //private IRedisService _redis;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(IUnitOfWork unitOfWork, IConfiguration config, IKafkaSender kafkaSender, ILogger<CustomerService> logger)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _kafkaSender = kafkaSender;
            _logger = logger;
        }

        public async Task<List<Customer>> GetAllCustomerAsync()
        {
            return await _unitOfWork.CustomerRepository.GetAll()
                .ToListAsync();
        }

        public async Task<Customer> GetCustomerByIdAsync(Guid CustomerId)
        {
            _logger.LogInformation("Getting All Data Customer");
            var result = await _unitOfWork.CustomerRepository.GetAll()
                .Where(x => x.CustomerId == CustomerId)
                .FirstOrDefaultAsync();
            return result;
        }

        public async Task CreateCustomerAsync(Customer data)
        {
            try
            {
                await _unitOfWork.CustomerRepository.AddAsync(data);
                await _unitOfWork.SaveAsync();
            }

            catch(Exception e)
            {

            }
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

    }

    public static class CustomerStatus
    {
        public const string Active = "Active";
        public const string InActive = "InActive";
        public const string NotFound = "NotFound";
    }
}
