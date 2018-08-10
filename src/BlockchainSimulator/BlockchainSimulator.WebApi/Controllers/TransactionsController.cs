using System;
using System.Collections.Generic;
using BlockchainSimulator.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainSimulator.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : BaseController
    {
        [HttpGet("{id}")]
        public ActionResult<Transaction> GetTransaction(string id)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult<List<Transaction>> GetPendingTransactions()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult<Transaction> AddTransaction([FromBody] Transaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}