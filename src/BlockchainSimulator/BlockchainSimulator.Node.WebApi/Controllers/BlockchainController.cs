using BlockchainSimulator.Node.BusinessLogic.Services;
using BlockchainSimulator.Node.WebApi.Models.Blockchain;
using Microsoft.AspNetCore.Mvc;

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
        private readonly IBlockchainService _blockchainService;

        /// <inheritdoc />
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="blockchainService">The blockchain repository</param>
        public BlockchainController(IBlockchainService blockchainService)
        {
            _blockchainService = blockchainService;
        }

        /// <summary>
        /// Gets the longest blockchain
        /// </summary>
        /// <returns>Longest blockchain</returns>
        [HttpGet]
        public ActionResult<object> GetLongestBlockchain()
        {
            return _blockchainService.GetLongestBlockchain();
        }

        /// <summary>
        /// Gets the locally stored blockchain
        /// </summary>
        /// <returns>The blockchain</returns>
        [HttpGet("tree")]
        public ActionResult<object> GetBlockchainTree()
        {
            return _blockchainService.GetBlockchainTree();
        }

        /// <summary>
        /// Gets last block
        /// </summary>
        /// <returns>The last block</returns>
        [HttpGet("last-block")]
        public ActionResult<object> GetLastBlock()
        {
            return _blockchainService.GetLastBlock();
        }

        /// <summary>
        /// Gets the block with given id
        /// </summary>
        /// <param name="id">The id of the block</param>
        /// <returns>The block with given id</returns>
        [HttpGet("{id}")]
        public ActionResult<object> GetBlock(string id)
        {
            return _blockchainService.GetBlock(id);
        }

        /// <summary>
        /// Gets the locally stored blockchain meta-data
        /// </summary>
        /// <returns>The blockchain</returns>
        [HttpGet("meta-data")]
        public ActionResult<BlockchainMetadata> GetBlockchainMetadata()
        {
            var metadata = _blockchainService.GetBlockchainMetadata();
            return LocalMapper.Map<BlockchainMetadata>(metadata);
        }
    }
}