using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace BlockchainSimulator.Node.WebApi.Controllers
{
    /// <summary>
    /// The info controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="configuration">The configuration</param>
        public InfoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Allows to ping the service and check its configuration
        /// </summary>
        /// <returns>Returns OK status</returns>
        [HttpGet]
        public List<KeyValuePair<string, string>> GetInfo()
        {
            return _configuration.AsEnumerable().Where(kv => kv.Value != null).ToList();
        }

        /// <summary>
        /// Stops all jobs in the service
        /// </summary>
        /// <returns>True if all services has been stopped</returns>
        [HttpPost]
        public bool StopAllJobs()
        {
            // TODO: Add service and add method to stop all jobs
            return true;
        }
    }
}