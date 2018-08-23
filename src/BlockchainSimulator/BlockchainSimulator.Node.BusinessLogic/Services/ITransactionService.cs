using System.Collections.Generic;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public interface ITransactionService
    {
        BaseResponse<Transaction> AddTransaction(Transaction transaction);

        BaseResponse<List<Transaction>> GetPendingTransactions();

        BaseResponse<Transaction> GetTransaction(string id);
    }
}