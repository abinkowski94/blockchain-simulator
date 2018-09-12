using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Model.Block;

namespace BlockchainSimulator.Node.DataAccess.Repositories
{
    public interface IBlockchainRepository
    {
        BlockBase GetLastBlock();

        BlockBase GetBlock(string id);

        BlockchainTree GetBlockchainTree();

        BlockchainTreeMetadata GetBlockchainMetadata();

        void AddBlock(BlockBase blockBase);

        bool BlockExists(string id);

        void SaveBlockchain(BlockchainTree blockchainTree);
    }
}