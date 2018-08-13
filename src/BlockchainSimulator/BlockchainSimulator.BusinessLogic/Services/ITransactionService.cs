using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Model.Transaction;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public interface ITransactionService
    {
        Transaction AddTransaction(Transaction transaction);

        List<Transaction> GetPendingTransactions();

        Transaction GetTransaction(string id);
    }
}