using System;
using System.Collections.Generic;
using System.Threading;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public interface IMiningService
    {
        void MineBlocks(IEnumerable<Transaction> transactions, DateTime enqueueTime, CancellationToken token);
    }
}