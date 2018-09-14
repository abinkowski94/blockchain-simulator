using System.Collections.Generic;
using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Model.Block;

namespace BlockchainSimulator.Node.DataAccess.Repositories
{
    public interface IBlockchainRepository
    {
        List<string> GetBlocksIds();

        List<BlockBase> GetBlocks(List<string> ids);
        
        BlockBase GetLastBlock();

        BlockBase GetBlock(string id);

        BlockchainTree GetBlockchainTree();

        BlockchainTree GetLongestBlockchain();

        BlockchainTreeMetadata GetBlockchainMetadata();

        void AddBlock(BlockBase blockBase);

        bool BlockExists(string uniqueId);

        void SaveBlockchain(BlockchainTree blockchainTree);
    }
}