using BlockchainSimulator.Common.Models.Responses;
using BlockchainSimulator.Node.BusinessLogic.Services;
using BlockchainSimulator.Node.WebApi.Extensions;
using BlockchainSimulator.Node.WebApi.Models.Transactions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BlockchainSimulator.Node.WebApi.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// The transaction controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : BaseController
    {
        private readonly ITransactionService _transactionService;

        /// <inheritdoc />
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="transactionService">The transaction service</param>
        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Adds transactions to the pending list
        /// </summary>
        /// <param name="transaction">The transaction</param>
        /// <returns>The newly added transaction</returns>
        [HttpPost]
        public ActionResult<BaseResponse> AddTransaction([FromBody] Transaction transaction)
        {
            var mappedTransaction = LocalMapper.Map<BusinessLogic.Model.Transaction.Transaction>(transaction);
            var result = _transactionService.AddTransaction(mappedTransaction);

            return result.GetActionResult<BusinessLogic.Model.Transaction.Transaction, Transaction>(this);
        }

        /// <summary>
        /// Adds transactions to the pending list
        /// </summary>
        /// <param name="transactions">The list of transactions</param>
        /// <returns>The newly added transactions</returns>
        [HttpPost("multiple")]
        public ActionResult<BaseResponse> AddTransactions([FromBody] List<Transaction> transactions)
        {
            var mappedTransactions = LocalMapper.Map<List<BusinessLogic.Model.Transaction.Transaction>>(transactions);
            var result = _transactionService.AddTransactions(mappedTransactions);

            return result.GetActionResult<List<BusinessLogic.Model.Transaction.Transaction>, List<Transaction>>(this);
        }

        /// <summary>
        /// Gets the list of pending transactions
        /// </summary>
        /// <returns>List of pending transactions</returns>
        [HttpGet]
        public ActionResult<BaseResponse> GetPendingTransactions()
        {
            var response = _transactionService.GetPendingTransactions();
            return response.GetActionResult<List<BusinessLogic.Model.Transaction.Transaction>, List<Transaction>>(this);
        }

        /// <summary>
        /// Gets a transaction with the specific id
        /// </summary>
        /// <param name="id">Id of the transaction</param>
        /// <returns>The transaction</returns>
        [HttpGet("{id}")]
        public ActionResult<BaseResponse> GetTransaction(string id)
        {
            var response = _transactionService.GetTransaction(id);
            return response.GetActionResult<BusinessLogic.Model.Transaction.Transaction, Transaction>(this);
        }
    }
}