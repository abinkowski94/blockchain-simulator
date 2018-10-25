using BlockchainSimulator.Node.BusinessLogic.Services;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace BlockchainSimulator.Node.BusinessLogic.Hubs
{
    public class ConsensusHub : Hub<IConsensusClient>
    {
        private readonly IBlockchainService _blockchainService;

        public ConsensusHub(IBlockchainService blockchainService)
        {
            _blockchainService = blockchainService;
        }

        public string GetBlocksFromBranchJson(string uniqueId)
        {
            return JsonConvert.SerializeObject(_blockchainService.GetBlockchainFromBranch(uniqueId).Blocks);
        }

        public string GetLastBlockJson()
        {
            return JsonConvert.SerializeObject(_blockchainService.GetLastBlock());
        }
    }
}