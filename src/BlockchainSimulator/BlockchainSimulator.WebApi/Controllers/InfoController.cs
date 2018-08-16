using Microsoft.AspNetCore.Mvc;

namespace BlockchainSimulator.WebApi.Controllers
{
    /// <summary>
    /// The info controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController
    {
        /// <summary>
        /// Allows to ping the service and check if is alive
        /// </summary>
        /// <returns>Returns OK status</returns>
        [HttpGet]
        public ActionResult GetInfo()
        {
            return new OkResult();
        }
    }
}