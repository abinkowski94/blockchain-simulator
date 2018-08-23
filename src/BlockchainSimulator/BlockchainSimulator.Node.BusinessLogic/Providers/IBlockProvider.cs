using System;
using System.Collections.Generic;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;

namespace BlockchainSimulator.Node.BusinessLogic.Providers
{
    public interface IBlockProvider
    {
        BlockBase CreateBlock(HashSet<Transaction> transactions, DateTime enqueueTime, BlockBase parentBlock = null);
    }
}