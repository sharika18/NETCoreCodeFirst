using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.AstraInternationHackerRank
{
    public class AstraInternationalHackerRankController : Controller
    {
        private AstraInternationHackerRankService _astraInternationalHackerRankService;
        public AstraInternationalHackerRankController(ILogger<AstraInternationalHackerRankController> logger)
        {

            _astraInternationalHackerRankService ??= new AstraInternationHackerRankService();
        }
        /// <summary>
        /// Get User Submissiom More Than Threshold
        /// </summary>
        /// <response code="200">Request ok.</response>
        [HttpGet]
        [Route("{threshold}")]
        [ProducesResponseType(typeof(List<string>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public ActionResult<List<string>> GetUserSubmissiomMoreThanThreshold([FromRoute] int threshold)
        {
            List<string> result = _astraInternationalHackerRankService.getUsernames(threshold);
            return new OkObjectResult(result);
        }
    }
}
