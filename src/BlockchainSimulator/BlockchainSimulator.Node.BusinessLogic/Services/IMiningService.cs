using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public interface IMiningService
    {
        void MineBlock(IEnumerable<Transaction> transactions, DateTime enqueueTime, CancellationToken token);
    }
}