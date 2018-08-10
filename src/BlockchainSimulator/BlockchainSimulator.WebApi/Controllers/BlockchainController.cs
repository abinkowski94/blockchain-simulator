using BlockchainSimulator.DataAccess.Model;
using BlockchainSimulator.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainSimulator.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlockchainController : BaseController
    {
        private readonly IBlockchainRepository _blockchainRepository;

        public BlockchainController(IBlockchainRepository blockchainRepository)
        {
            _blockchainRepository = blockchainRepository;
        }

        [HttpGet]
        public ActionResult<Blockchain> GetBlockchain()
        {
            return _blockchainRepository.GetBlockchain();
        }
    }
}