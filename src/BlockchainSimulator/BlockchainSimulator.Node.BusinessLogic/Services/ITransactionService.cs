using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public interface ITransactionService
    {
        ConcurrentDictionary<string, Transaction> RegisteredTransactions { get; }

        BaseResponse<Transaction> AddTransaction(Transaction transaction);

        BaseResponse<List<Transaction>> GetPendingTransactions();

        BaseResponse<Transaction> GetTransaction(string id);

        BaseResponse<List<Transaction>> AddTransactions(List<Transaction> transactions);

        BaseResponse<bool> ForceMining();
        
        void Clear();
    }
}