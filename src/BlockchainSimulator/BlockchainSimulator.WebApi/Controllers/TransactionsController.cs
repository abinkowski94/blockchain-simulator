using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Services;
using BlockchainSimulator.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainSimulator.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : BaseController
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("{id}")]
        public ActionResult<Transaction> GetTransaction(string id)
        {
            var transaction = _transactionService.GetTransaction(id);
            return LocalMapper.Map<Transaction>(transaction);
        }

        [HttpGet]
        public ActionResult<List<Transaction>> GetPendingTransactions()
        {
            var transactions = _transactionService.GetPendingTransactions();
            return LocalMapper.Map<List<Transaction>>(transactions);
        }

        [HttpPost]
        public ActionResult<Transaction> AddTransaction([FromBody] Transaction transaction)
        {
            var mappedTransaction = LocalMapper.Map<BusinessLogic.Model.Transaction.Transaction>(transaction);
            var result = _transactionService.AddTransaction(mappedTransaction);

            return LocalMapper.Map<Transaction>(result);
        }
    }
}