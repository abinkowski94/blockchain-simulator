using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Services;
using BlockchainSimulator.WebApi.Extensions;
using BlockchainSimulator.WebApi.Models;
using BlockchainSimulator.WebApi.Models.Responses;
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
        public ActionResult<BaseResponse> GetTransaction(string id)
        {
            var response = _transactionService.GetTransaction(id);
            return response.GetBaseResponse<BusinessLogic.Model.Transaction.Transaction, Transaction>(this);
        }

        [HttpGet]
        public ActionResult<BaseResponse> GetPendingTransactions()
        {
            var response = _transactionService.GetPendingTransactions();
            return response.GetBaseResponse<List<BusinessLogic.Model.Transaction.Transaction>, List<Transaction>>(this);
        }

        [HttpPost]
        public ActionResult<BaseResponse> AddTransaction([FromBody] Transaction transaction)
        {
            var mappedTransaction = LocalMapper.Map<BusinessLogic.Model.Transaction.Transaction>(transaction);
            var result = _transactionService.AddTransaction(mappedTransaction);

            return result.GetBaseResponse<BusinessLogic.Model.Transaction.Transaction, Transaction>(this);
        }
    }
}