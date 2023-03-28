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
using BLL.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalesController : ControllerBase
    {
        private ISalesService _salesService;
        private IMapper _mapper;
        private readonly ILogger<SalesController> _logger;
        public SalesController
            (
                ILogger<SalesController> logger, 
                IMapper mapper, 
                ISalesService salesService
            )
        {
            _mapper = mapper;
            _logger = logger;
            _salesService = salesService;

        }

        /// <summary>
        /// Get all Sales
        /// </summary>
        /// <response code="200">Request ok.</response>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<SalesWithDependencyDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> GetAllAsync()
        {
            //List<string> resultt = _SalesService.getUsernames(10);
            List<Model.Sales> result = await _salesService.GetAllSalesAsync();
            List<SalesWithDependencyDTO> mappedResult = _mapper.Map<List<SalesWithDependencyDTO>>(result);
            return new OkObjectResult(mappedResult);
        }

        /// <summary>
        /// Get Sales by Id
        /// </summary>
        /// <response code="200">Request ok.</response>
        [HttpGet]
        [Route("{SalesId}")]
        [ProducesResponseType(typeof(List<SalesWithDependencyDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> GetByIdAsync([FromRoute] Guid SalesId)
        {
            Model.Sales result = await _salesService.GetSalesByIdAsync(SalesId);
            if (result != null)
            {
                SalesWithDependencyDTO mappedResult = _mapper.Map<SalesWithDependencyDTO>(result);
                return new OkObjectResult(mappedResult);
            }

            return new NotFoundResult();
        }

        /// <summary>
        /// Create Sales 
        /// </summary>
        /// <param name="SalesDTO">Sales data.</param>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(SalesDTO), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> CreateAsync([FromBody] SalesCreateDTO SalesCreateDTO)
        {
            try
            {
                _logger.LogInformation($"Create Sales/Order Started");
                Model.Sales data = _mapper.Map<Model.Sales>(SalesCreateDTO);
                await _salesService.CreateSalesAsync(data);
                return new OkResult();
            }
            catch(Exception e)
            {
                return new BadRequestObjectResult(e.Message.ToString());
            }
        }

        /// <summary>
        /// Update Sales 
        /// </summary>
        /// <param name="SalesDTO">Sales data.</param>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpPut]
        [Route("")]
        [ProducesResponseType(typeof(SalesDTO), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> UpdateAscync([FromBody] SalesCreateDTO SalesDTO)
        {
            try
            {
                Model.Sales data = _mapper.Map<Model.Sales>(SalesDTO);
                await _salesService.UpdateSalesAsync(data);
                return new OkResult();
            }
            catch(Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }

        }

        /// <summary>
        /// Get all Territories
        /// </summary>
        /// <response code="200">Request ok.</response>
        [HttpGet]
        [Route("Territories")]
        [ProducesResponseType(typeof(List<SalesWithDependencyDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> GetAllTerritoriesAsync()
        {
            //List<string> resultt = _SalesService.getUsernames(10);
            List<Model.Territories> result = await _salesService.GetAllTerritoriesAsync();
            List<TerritoriesDTO> mappedResult = _mapper.Map<List<TerritoriesDTO>>(result);
            return new OkObjectResult(mappedResult);
        }

        /// <summary>
        /// Create Territories 
        /// </summary>
        /// <param name="TerritoriesDTO">Territories data.</param>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpPost]
        [Route("Territories")]
        [ProducesResponseType(typeof(TerritoriesDTO), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> CreateAsync([FromBody] TerritoriesCreateDTO TerritoriesCreateDTO)
        {
            try
            {
                Model.Territories data = _mapper.Map<Model.Territories>(TerritoriesCreateDTO);
                await _salesService.CreateTerritoriesAsync(data);
                return new OkResult();
            }
            catch
            {
                return new BadRequestResult();
            }
        }
    }
}
