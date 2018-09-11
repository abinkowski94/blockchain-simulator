using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Model.Block;

namespace BlockchainSimulator.Node.DataAccess.Repositories
{
    public interface IBlockchainRepository
    {
        Blockchain GetBlockchain();

        BlockBase GetLastBlock();

        BlockBase GetBlock(string id);
        
        BlockchainMetadata GetBlockchainMetadata();

        void SaveBlockchain(Blockchain blockchain);
    }
}