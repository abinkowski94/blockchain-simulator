using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public interface IMiningService
    {
        int AbandonedBlocksCount { get; }
        int MiningAttemptsCount { get; }

        void MineBlocks(IEnumerable<Transaction> transactions, DateTime enqueueTime, CancellationToken token);
    }
}