using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.Transaction;

namespace BlockchainSimulator.BusinessLogic.Providers
{
    public interface IBlockProvider
    {
        Block CreateBlock(HashSet<Transaction> transactions, Block parentBlock);
    }
}