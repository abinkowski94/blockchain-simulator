using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Providers
{
    public interface IBlockProvider
    {
        Task<BlockBase> CreateBlock(HashSet<Transaction> transactions, DateTime enqueueTime,
            BlockBase parentBlock = null,
            CancellationToken token = default(CancellationToken));
    }
}