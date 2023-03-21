﻿using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductAsync();
        Task<Product> GetProductByIdAsync(Guid id);
        //Task CreateProductAsync(Product data);
        //Task UpdateProductAsync(Product data);
        //Task DeleteProductAsync(Guid ProductId);
    }
}
