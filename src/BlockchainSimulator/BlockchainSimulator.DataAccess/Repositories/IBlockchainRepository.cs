using BlockchainSimulator.DataAccess.Model;

namespace BlockchainSimulator.DataAccess.Repositories
{
    public interface IBlockchainRepository
    {
        Blockchain GetBlockchain();

        Blockchain SaveBlockchain(Blockchain blockchain);
    }
}