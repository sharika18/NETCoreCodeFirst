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
    public class ProductService : IProductService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IRedisService _redis;
        private readonly ILogger<ProductService> _logger;
        public ProductService(
            IUnitOfWork unitOfWork, 
            ILogger<ProductService> logger, 
            IRedisService redis)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _redis = redis;
        }

        public async Task<List<Product>> GetAllProductAsync()
        {
            return await _unitOfWork.ProductRepository.GetAll()
                .Include(a => a.SubCategory)
                .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(Guid ProductId)
        {
            _logger.LogInformation("Getting Product By Id");
            Product product = await _redis.GetAsync<Product>($"{PrefixRedisKey.ProductKey}:{ProductId}");

            if (product == null)
            {
                product = await _unitOfWork.ProductRepository.GetAll()
                    .Include(a => a.SubCategory)
                    .Where(x => x.ProductId == ProductId)
                    .FirstOrDefaultAsync();

                TimeSpan expirition = new TimeSpan(1, 0, 00);
                await _redis.SaveAsync($"{PrefixRedisKey.ProductKey}:{ProductId}", product, expirition);
            }

            return product;
        }

        public async Task CreateProductAsync(Product data)
        {
            await _unitOfWork.ProductRepository.AddAsync(data);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateProductAsync(Product data)
        {
            bool isExist = _unitOfWork.ProductRepository.IsExist(x => x.ProductId == data.ProductId);

            if (!isExist)
            {
                throw new Exception($"Product with id {data.ProductId} not exist");

            }
            _unitOfWork.ProductRepository.Edit(data);
            await _unitOfWork.SaveAsync();
            await _redis.DeleteAsync($"{PrefixRedisKey.ProductKey}:{data.ProductId}");
        }

        public async Task DeleteProductAsync(Guid ProductId)
        {
            var isExist = _unitOfWork.ProductRepository.IsExist(x => x.ProductId == ProductId);
            if (!isExist)
            {
                throw new Exception($"Product with id {ProductId} not exist");
            }

            _unitOfWork.ProductRepository.Delete(x => x.ProductId == ProductId);
            await _unitOfWork.SaveAsync();
            await _redis.DeleteAsync($"{PrefixRedisKey.ProductKey}:{ProductId}");
        }

    }
}
