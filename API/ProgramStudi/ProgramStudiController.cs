using AutoMapper;
using AutoMapper.QueryableExtensions;
using BLL.Services;
using DAL.Repositories;
using API.ProgramStudi.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Model = DAL.Model;
using Microsoft.Extensions.Logging;

namespace API.Fakultas
{
    [ApiController]
    [Route("[controller]")]
    public class ProgramStudiController : ControllerBase
    {
        private ProgramStudiService _programStudiService;
        //private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<ProgramStudiService> _Ilogger;
        public ProgramStudiController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProgramStudiService> logger)
        {
            //_unitOfWork = unitOfWork;
            _mapper = mapper;
            _Ilogger = logger;

            _programStudiService ??= new ProgramStudiService(unitOfWork, _Ilogger);
        }

        /// <summary>
        /// Get all ProgramStudi
        /// </summary>
        /// <response code="200">Request ok.</response>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<ProgramStudiWithFakultasDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> GetAllAsync()
        {
            List<Model.ProgramStudi> result = await _programStudiService.GetAllProgramStudiAsync();
            List<ProgramStudiWithFakultasDTO> mappedResult = _mapper.Map<List<ProgramStudiWithFakultasDTO>>(result);
            return new OkObjectResult(mappedResult);
        }

        /// <summary>
        /// Get ProgramStudi by Id
        /// </summary>
        /// <response code="200">Request ok.</response>
        [HttpGet]
        [Route("{ProgramStudiId}")]
        [ProducesResponseType(typeof(List<ProgramStudiDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> GetByIdAsync([FromRoute] Guid ProgramStudiId)
        {
            Model.ProgramStudi result = await _programStudiService.GetProgramStudiByIdAsync(ProgramStudiId);
            if (result != null)
            {
                ProgramStudiDTO mappedResult = _mapper.Map<ProgramStudiDTO>(result);
                return new OkObjectResult(mappedResult);
            }

            return new NotFoundResult();
        }

        /// <summary>
        /// Create ProgramStudi 
        /// </summary>
        /// <param name="programStudiDTO">ProgramStudi data.</param>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(ProgramStudiDTO), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> CreateAsync([FromBody] ProgramStudiWithFakultasDTO programStudiDTO)
        {
            try
            {
                Model.ProgramStudi data = _mapper.Map<Model.ProgramStudi>(programStudiDTO);
                await _programStudiService.CreateProgramStudiAsync(data);
                return new OkResult();
            }
            catch(Exception e)
            {
                return new BadRequestObjectResult(e.InnerException.Message.ToString());
            }
        }

        /// <summary>
        /// Update ProgramStudi 
        /// </summary>
        /// <param name="ProgramStudiDTO">ProgramStudi data.</param>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpPut]
        [Route("")]
        [ProducesResponseType(typeof(ProgramStudiDTO), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> UpdateAscync([FromBody] ProgramStudiWithFakultasDTO ProgramStudiDTO)
        {
            try
            {
                Model.ProgramStudi data = _mapper.Map<Model.ProgramStudi>(ProgramStudiDTO);
                await _programStudiService.UpdateProgramStudiAsync(data);
                return new OkResult();
            }
            catch
            {
                return new BadRequestResult();
            }

        }

        /// <summary>
        /// Delete ProgramStudi
        /// </summary>
        /// <param name="programStudiId">ProgramStudi data.</param>
        /// <response code="200">Request ok.</response>
        [HttpDelete]
        [Route("{programStudiId}")]
        [ProducesResponseType(typeof(ProgramStudiDTO), 200)]
        public async Task<ActionResult> Delete([FromRoute] Guid programStudiId)
        {
            try
            {
                await _programStudiService.DeleteProgramStudiAsync(programStudiId);
                return new OkResult();
            }
            catch
            {
                return new BadRequestResult();
            }
        }
    }
}
