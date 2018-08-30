using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BlockchainSimulator.Hub.WebApi.Controllers
{
    /// <summary>
    /// The info controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
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
    }
}