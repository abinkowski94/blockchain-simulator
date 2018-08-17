using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlockchainSimulator.BusinessLogic.Model.Transaction;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public interface IMiningService
    {
        Task MineBlocks(IEnumerable<Transaction> transactions, DateTime enqueueTime, CancellationToken token);
    }
}