using AutoMapper;
using BLL.Kafka;
using BLL.Services;
using DAL.Repositories;
using API.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Model = DAL.Models;
using BLL.Cache;
using BLL.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private IProductService _productService;
        private IMapper _mapper;
        private readonly ILogger<ProductController> _logger;
        public ProductController(
            ILogger<ProductController> logger, 
            IMapper mapper, 
            IProductService productService)
        {
            _mapper = mapper;
            _logger = logger;

            _productService = productService;
        }

        /// <summary>
        /// Get all Product
        /// </summary>
        /// <response code="200">Request ok.</response>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<ProductWithSubCategoryDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> GetAllAsync()
        {
            //List<string> resultt = _ProductService.getUsernames(10);
            List<Model.Product> result = await _productService.GetAllProductAsync();
            List<ProductWithSubCategoryDTO> mappedResult = _mapper.Map<List<ProductWithSubCategoryDTO>>(result);
            return new OkObjectResult(mappedResult);
        }

        /// <summary>
        /// Get Product by Id
        /// </summary>
        /// <response code="200">Request ok.</response>
        [HttpGet]
        [Route("{ProductId}")]
        [ProducesResponseType(typeof(List<ProductWithSubCategoryDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> GetByIdAsync([FromRoute] Guid ProductId)
        {
            Model.Product result = await _productService.GetProductByIdAsync(ProductId);
            if (result != null)
            {
                ProductWithSubCategoryDTO mappedResult = _mapper.Map<ProductWithSubCategoryDTO>(result);
                return new OkObjectResult(mappedResult);
            }

            return new NotFoundResult();
        }


        ///// <summary>
        ///// Create Product 
        ///// </summary>
        ///// <param name="ProductDTO">Product data.</param>
        ///// <response code="200">Request ok.</response>
        ///// <response code="400">Request failed because of an exception.</response>
        //[HttpPost]
        //[Route("")]
        //[ProducesResponseType(typeof(ProductDTO), 200)]
        //[ProducesResponseType(typeof(string), 400)]
        //public async Task<ActionResult> CreateAsync([FromBody] ProductCreateDTO ProductCreateDTO)
        //{
        //    try
        //    {
        //        Model.Product data = _mapper.Map<Model.Product>(ProductCreateDTO);
        //        await _productService.CreateProductAsync(data);
        //        return new OkResult();
        //    }
        //    catch
        //    {
        //        return new BadRequestResult();
        //    }
        //}

        ///// <summary>
        ///// Update Product 
        ///// </summary>
        ///// <param name="ProductDTO">Product data.</param>
        ///// <response code="200">Request ok.</response>
        ///// <response code="400">Request failed because of an exception.</response>
        //[HttpPut]
        //[Route("")]
        //[ProducesResponseType(typeof(ProductDTO), 200)]
        //[ProducesResponseType(typeof(string), 400)]
        //public async Task<ActionResult> UpdateAscync([FromBody] ProductDTO ProductDTO)
        //{
        //    try
        //    {
        //        Model.Product data = _mapper.Map<Model.Product>(ProductDTO);
        //        await _productService.UpdateProductAsync(data);
        //        return new OkResult();
        //    }
        //    catch
        //    {
        //        return new BadRequestResult();
        //    }

        //}

        ///// <summary>
        ///// Delete Product
        ///// </summary>
        ///// <param name="ProductId">Product data.</param>
        ///// <response code="200">Request ok.</response>
        //[HttpDelete]
        //[Route("{ProductId}")]
        //[ProducesResponseType(typeof(ProductDTO), 200)]
        //public async Task<ActionResult> DeleteAsync([FromRoute] Guid ProductId)
        //{
        //    try
        //    {
        //        await _productService.DeleteProductAsync(ProductId);
        //        return new OkResult();
        //    }
        //    catch
        //    {
        //        return new BadRequestResult();
        //    }

        //}
    }
}
