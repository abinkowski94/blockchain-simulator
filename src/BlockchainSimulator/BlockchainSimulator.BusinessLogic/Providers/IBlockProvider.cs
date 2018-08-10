using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.Transaction;

namespace BlockchainSimulator.BusinessLogic.Providers
{
    public interface IBlockProvider
    {
        BlockBase CreateBlock(HashSet<Transaction> transactions, BlockBase parentBlock = null);
    }
}