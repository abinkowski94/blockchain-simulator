using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public interface IMiningService
    {
        Task MineBlocks(IEnumerable<Transaction> transactions, DateTime enqueueTime, CancellationToken token);
    }
}