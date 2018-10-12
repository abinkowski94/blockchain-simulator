using System.Collections.Concurrent;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;

namespace BlockchainSimulator.Node.BusinessLogic.Storage
{
    public interface ITransactionStorage
    {
        ConcurrentDictionary<string, Transaction> PendingTransactions { get; }
        ConcurrentDictionary<string, Transaction> RegisteredTransactions { get; }
        void Clear();
    }
}