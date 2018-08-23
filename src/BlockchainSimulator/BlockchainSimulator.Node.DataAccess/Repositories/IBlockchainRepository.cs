using BlockchainSimulator.Node.DataAccess.Model;

namespace BlockchainSimulator.Node.DataAccess.Repositories
{
    public interface IBlockchainRepository
    {
        Blockchain GetBlockchain();

        Blockchain SaveBlockchain(Blockchain blockchain);
    }
}