using BlockchainSimulator.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BlockchainSimulator.WebApi.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// The blockchain controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BlockchainController : BaseController
    {
        private readonly IBlockchainRepository _blockchainRepository;

        /// <inheritdoc />
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="blockchainRepository">The blockchain repository</param>
        public BlockchainController(IBlockchainRepository blockchainRepository)
        {
            _blockchainRepository = blockchainRepository;
        }

        /// <summary>
        /// Gets the locally stored blockchain
        /// </summary>
        /// <returns>The blockchain</returns>
        [HttpGet]
        public ActionResult<string> GetBlockchain()
        {
            return JsonConvert.SerializeObject(_blockchainRepository.GetBlockchain());
        }
    }
}