using BlockchainSimulator.Node.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BlockchainSimulator.Node.WebApi.Controllers
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
        /// Gets the block with given id
        /// </summary>
        /// <param name="id">The id of the block</param>
        /// <returns>The block with given id</returns>
        [HttpGet("{id}")]
        public ActionResult<object> GetBlock(string id)
        {
            return _blockchainRepository.GetBlockchain()?.Blocks?.FirstOrDefault(b => b.Id == id);
        }

        /// <summary>
        /// Gets the locally stored blockchain
        /// </summary>
        /// <returns>The blockchain</returns>
        [HttpGet]
        public ActionResult<object> GetBlockchain()
        {
            return _blockchainRepository.GetBlockchain();
        }
    }
}