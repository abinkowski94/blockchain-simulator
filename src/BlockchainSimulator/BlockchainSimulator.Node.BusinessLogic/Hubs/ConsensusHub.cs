using BlockchainSimulator.Node.DataAccess.Repositories;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace BlockchainSimulator.Node.BusinessLogic.Hubs
{
    public class ConsensusHub : Hub<IConsensusClient>
    {
        private readonly IBlockchainRepository _blockchainRepository;

        public ConsensusHub(IBlockchainRepository blockchainRepository)
        {
            _blockchainRepository = blockchainRepository;
        }

        public string GetBlocksFromBranchJson(string uniqueId)
        {
            return JsonConvert.SerializeObject(_blockchainRepository.GetBlockchainFromBranch(uniqueId).Blocks);
        }

        public string GetLastBlockJson()
        {
            return JsonConvert.SerializeObject(_blockchainRepository.GetLastBlock());
        }
    }
}