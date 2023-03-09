using AutoMapper;
using BLL.Kafka;
using BLL.Services;
using DAL.Repositories;
using API.Falultas.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Model = DAL.Model;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FakultasController : ControllerBase
    {
        private FakultasService _fakultasService;
        private IMapper _mapper;
        private readonly ILogger<FakultasController> _logger;
        public FakultasController(ILogger<FakultasController> logger, IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, IKafkaSender kafkaSender, ILogger<FakultasService> _loggerFakultasService)
        {
            _mapper = mapper;
            _logger = logger;

            _fakultasService ??= new FakultasService(unitOfWork, configuration, kafkaSender, _loggerFakultasService);
        }

        /// <summary>
        /// Get all fakultas
        /// </summary>
        /// <response code="200">Request ok.</response>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<FakultasWithProgramStudiDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> GetAllAsync()
        {
            //List<string> resultt = _fakultasService.getUsernames(10);
            List<Model.Fakultas> result = await _fakultasService.GetAllFakultasAsync();
            List<FakultasWithProgramStudiDTO> mappedResult = _mapper.Map<List<FakultasWithProgramStudiDTO>>(result);
            return new OkObjectResult(mappedResult);
        }

        /// <summary>
        /// Get fakultas by Id
        /// </summary>
        /// <response code="200">Request ok.</response>
        [HttpGet]
        [Route("{fakultasId}")]
        [ProducesResponseType(typeof(List<FakultasWithProgramStudiDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> GetByIdAsync([FromRoute] Guid fakultasId)
        {
            Model.Fakultas result = await _fakultasService.GetFakultasByIdAsync(fakultasId);
            if(result != null)
            {
                FakultasWithProgramStudiDTO mappedResult = _mapper.Map<FakultasWithProgramStudiDTO>(result);
                return new OkObjectResult(mappedResult);
            }

            return new NotFoundResult();
        }

        /// <summary>
        /// Create fakultas 
        /// </summary>
        /// <param name="FakultasDTO">Fakultas data.</param>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(FakultasDTO), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> CreateAsync([FromBody] FakultasDTO fakultasDTO)
        {
            try
            {
                Model.Fakultas data = _mapper.Map<Model.Fakultas>(fakultasDTO);
                await _fakultasService.CreateFakultasAsync(data);
                return new OkResult();
            }
            catch
            {
                return new BadRequestResult();
            }
        }

        /// <summary>
        /// Update fakultas 
        /// </summary>
        /// <param name="FakultasDTO">Fakultas data.</param>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpPut]
        [Route("")]
        [ProducesResponseType(typeof(FakultasDTO), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> UpdateAscync([FromBody] FakultasDTO fakultasDTO)
        {
            try 
            {
                Model.Fakultas data = _mapper.Map<Model.Fakultas>(fakultasDTO);
                data = await _fakultasService.UpdateFakultasAsync(data);
                FakultasDTO mappedResult = _mapper.Map<FakultasDTO>(data);
                return new OkObjectResult(mappedResult);
            }
            catch
            {
                return new BadRequestResult();
            }

        }

        /// <summary>
        /// Delete Fakultas
        /// </summary>
        /// <param name="fakultasId">Fakultas data.</param>
        /// <response code="200">Request ok.</response>
        [HttpDelete]
        [Route("{fakultasId}")]
        [ProducesResponseType(typeof(FakultasDTO), 200)]
        public async Task<ActionResult> DeleteAsync([FromRoute] Guid fakultasId)
        {
            try 
            {
                await _fakultasService.DeleteFakultasAsync(fakultasId);
                return new OkResult();
            }
            catch
            {
                return new BadRequestResult();
            }
        
        }
    }
}
