using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using BlockchainSimulator.Common.Models.Responses;
using BlockchainSimulator.Node.BusinessLogic.Services;
using BlockchainSimulator.Node.WebApi.Extensions;

namespace BlockchainSimulator.Node.WebApi.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// The info controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : BaseController
    {
        /// <summary>
        /// The configuration service
        /// </summary>
        private readonly IConfigurationService _configurationService;

        /// <inheritdoc />
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="configurationService">The configuration service</param>
        public InfoController(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        /// <summary>
        /// Allows to ping the service and check its configuration
        /// </summary>
        /// <returns>Returns OK status</returns>
        [HttpGet]
        public List<KeyValuePair<string, string>> GetInfo()
        {
            return _configurationService.GetConfigurationInfo();
        }

        /// <summary>
        /// Stops all jobs in the service
        /// </summary>
        /// <returns>True if all services has been stopped</returns>
        [HttpPost]
        public ActionResult<BaseResponse> StopAllJobs()
        {
            return _configurationService.StopAllJobs().GetActionResult<bool, bool>(this);
        }
    }
}