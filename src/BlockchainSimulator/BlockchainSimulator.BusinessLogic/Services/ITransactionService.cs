using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Model.Responses;
using BlockchainSimulator.BusinessLogic.Model.Transaction;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public interface ITransactionService
    {
        BaseResponse<Transaction> AddTransaction(Transaction transaction);

        BaseResponse<List<Transaction>> GetPendingTransactions();

        BaseResponse<Transaction> GetTransaction(string id);
    }
}