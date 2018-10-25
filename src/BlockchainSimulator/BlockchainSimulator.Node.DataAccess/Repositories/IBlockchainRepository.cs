using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Model.Block;
using Microsoft.Extensions.Configuration;

namespace BlockchainSimulator.Node.DataAccess.Repositories
{
    public interface IBlockchainRepository
    {
        void CreateGenesisBlock(IConfiguration configuration);
        
        BlockBase GetBlock(string uniqueId);

        BlockBase GetLastBlock();

        BlockchainTreeMetadata GetBlockchainMetadata();

        BlockchainTree GetBlockchainFromBranch(string uniqueId);

        BlockchainTree GetLongestBlockchain();

        BlockchainTree GetBlockchainTree();

        void AddBlock(BlockBase blockBase);

        bool BlockExists(string uniqueId);

        void Clear();
    }
}