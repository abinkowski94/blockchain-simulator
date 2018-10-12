using System.Collections.Concurrent;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;

namespace BlockchainSimulator.Node.BusinessLogic.Storage
{
    public class TransactionStorage : ITransactionStorage
    {
        public ConcurrentDictionary<string, Transaction> PendingTransactions { get; }
        public ConcurrentDictionary<string, Transaction> RegisteredTransactions { get; }
        
        public void Clear()
        {
            PendingTransactions.Clear();
            RegisteredTransactions.Clear();
        }

        public TransactionStorage()
        {
            RegisteredTransactions = new ConcurrentDictionary<string, Transaction>();
            PendingTransactions = new ConcurrentDictionary<string, Transaction>();
        }
    }
}