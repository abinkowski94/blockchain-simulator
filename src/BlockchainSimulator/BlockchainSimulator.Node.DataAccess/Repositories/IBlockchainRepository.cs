using BlockchainSimulator.Node.DataAccess.Model;

namespace BlockchainSimulator.Node.DataAccess.Repositories
{
    public interface IBlockchainRepository
    {
        Blockchain GetBlockchain();

        BlockchainMetadata GetBlockchainMetadata();

        bool SaveBlockchain(Blockchain blockchain);
    }
}