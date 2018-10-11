using BlockchainSimulator.Common.Models;
using BlockchainSimulator.Common.Models.Responses;
using BlockchainSimulator.Node.BusinessLogic.Services;
using BlockchainSimulator.Node.WebApi.Extensions;
using Microsoft.AspNetCore.Mvc;

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
        public BlockchainNodeConfiguration GetConfiguration()
        {
            return _configurationService.GetConfiguration();
        }

        /// <summary>
        /// Changes the configuration of the node and restarts it
        /// </summary>
        /// <param name="configuration">The new configuration</param>
        /// <returns>The response whether the operation succeeded</returns>
        [HttpPost("config")]
        public ActionResult<BaseResponse> ClearAndChangeConfiguration([FromBody] BlockchainNodeConfiguration configuration)
        {
            return _configurationService.ChangeConfiguration(configuration).GetActionResult<bool, bool>(this);
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

        /// <summary>
        /// Wipes the node blockchain, other nodes connection, etc. completely
        /// </summary>
        /// <returns>Response if operation succeeded</returns>
        [HttpPost("clear")]
        public ActionResult<BaseResponse> ClearNode()
        {
            return _configurationService.ClearNode().GetActionResult<bool, bool>(this);
        }
    }
}