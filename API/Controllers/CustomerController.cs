using API.DTO;
using AutoMapper;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = DAL.Models;
namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : Controller
    {
        private ICustomerService _customerService;
        private IMapper _mapper;
        private readonly ILogger<CustomerController> _logger;
        public CustomerController(
            ILogger<CustomerController> logger,
            IMapper mapper,
            ICustomerService customerService)
        {
            _mapper = mapper;
            _logger = logger;

            _customerService = customerService;
        }

        /// <summary>
        /// Get all Customer
        /// </summary>
        /// <response code="200">Request ok.</response>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<CustomerDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> GetAllAsync()
        {
            List<Model.Customer> result = await _customerService.GetAllCustomerAsync();
            List<CustomerDTO> mappedResult = _mapper.Map<List<CustomerDTO>>(result);
            return new OkObjectResult(mappedResult);
        }
    }
}
