using Microsoft.AspNetCore.Mvc;

namespace BlockchainSimulator.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController
    {
        [HttpGet]
        public ActionResult GetInfo()
        {
            return new OkResult();
        }
    }
}