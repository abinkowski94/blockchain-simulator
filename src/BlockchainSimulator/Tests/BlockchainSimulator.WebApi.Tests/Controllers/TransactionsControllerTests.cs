using System;
using System.Collections.Generic;
using System.Linq;
using BlockchainSimulator.BusinessLogic.Model.Responses;
using BlockchainSimulator.BusinessLogic.Model.Transaction;
using BlockchainSimulator.BusinessLogic.Services;
using BlockchainSimulator.WebApi.Controllers;
using Moq;
using Xunit;

namespace BlockchainSimulator.WebApi.Tests.Controllers
{
    public class TransactionsControllerTests
    {
        private readonly Mock<ITransactionService> _transactionServiceMock;
        private readonly TransactionsController _transactionsController;

        public TransactionsControllerTests()
        {
            _transactionServiceMock = new Mock<ITransactionService>();
            _transactionsController = new TransactionsController(_transactionServiceMock.Object);
        }

        [Fact]
        public void GetTransaction_Id_Transaction()
        {
            // Arrange
            const string id = "898f8c48-da7e-4996-9a40-e6d6ddecdf48";
            var businessTransaction = new Transaction
            {
                Id = id,
                Sender = "4c9395c5-07d9-42c2-9fe9-19478b312acf",
                Recipient = "75f05331-a972-4fa5-b095-0672a8f2c537",
                Amount = 10,
                Fee = 0.5m,
                TransactionDetails = new TransactionDetails
                {
                    BlockId = "898f8c48-da7e-4996-b095-0672a8f2c537",
                    BlocksBehind = 2,
                    IsConfirmed = true
                }
            };

            _transactionServiceMock.Setup(p => p.GetTransaction(id))
                .Returns(new SuccessResponse<Transaction>("", businessTransaction));

            // Act
            var result = _transactionsController.GetTransaction(id);
            var transaction = result.Value.Result as Models.Transaction;

            // Assert
            _transactionServiceMock.Verify(p => p.GetTransaction(id));

            Assert.NotNull(result);
            Assert.NotNull(transaction);
            Assert.Equal(id, transaction.Id);
            Assert.Equal(businessTransaction.Sender, transaction.Sender);
            Assert.Equal(businessTransaction.Recipient, transaction.Recipient);
            Assert.Equal(businessTransaction.Amount, transaction.Amount);
            Assert.Equal(businessTransaction.Fee, transaction.Fee);
            Assert.Equal(businessTransaction.TransactionDetails.IsConfirmed,
                transaction.TransactionDetails.IsConfirmed);
            Assert.Equal(businessTransaction.TransactionDetails.BlockId, transaction.TransactionDetails.BlockId);
            Assert.Equal(businessTransaction.TransactionDetails.BlocksBehind,
                transaction.TransactionDetails.BlocksBehind);
        }

        [Fact]
        public void GetTransaction_Null_Error()
        {
            // Arrange
            _transactionServiceMock.Setup(p => p.GetTransaction(null))
                .Returns(new ErrorResponse<Transaction>("The id cannot be null!", null, "Inner error"));

            // Act
            var result = _transactionsController.GetTransaction(null);
            var errorResponse = result.Value as Models.Responses.ErrorResponse;
            var transaction = result.Value.Result as Models.Transaction;

            // Assert
            _transactionServiceMock.Verify(p => p.GetTransaction(null));

            Assert.NotNull(result);
            Assert.NotNull(errorResponse);
            Assert.NotNull(result.Value);
            Assert.Null(transaction);
            Assert.Equal("The id cannot be null!", result.Value.Message);
            Assert.Equal("Inner error", errorResponse.Errors.First());
        }


        [Fact]
        public void GetPendingTransactions_Empty_TransactionList()
        {
            // Arrange
            var businessTransaction = new Transaction
            {
                Id = "898f8c48-da7e-4996-9a40-e6d6ddecdf48",
                Sender = "4c9395c5-07d9-42c2-9fe9-19478b312acf",
                Recipient = "75f05331-a972-4fa5-b095-0672a8f2c537",
                Amount = 10,
                Fee = 0.5m,
                TransactionDetails = new TransactionDetails
                {
                    BlockId = "898f8c48-da7e-4996-b095-0672a8f2c537",
                    BlocksBehind = 2,
                    IsConfirmed = true
                }
            };

            _transactionServiceMock.Setup(p => p.GetPendingTransactions())
                .Returns(new SuccessResponse<List<Transaction>>("",
                    new List<Transaction> {businessTransaction, businessTransaction}));

            // Act
            var result = _transactionsController.GetPendingTransactions();
            var transactions = result.Value.Result as List<Models.Transaction>;
            var transaction = transactions?.First();

            // Assert
            _transactionServiceMock.Verify(p => p.GetPendingTransactions());

            Assert.NotNull(result);
            Assert.NotNull(transactions);
            Assert.Equal(2, transactions.Count);
            Assert.NotNull(transaction);
            Assert.Equal(businessTransaction.Id, transaction.Id);
            Assert.Equal(businessTransaction.Sender, transaction.Sender);
            Assert.Equal(businessTransaction.Recipient, transaction.Recipient);
            Assert.Equal(businessTransaction.Amount, transaction.Amount);
            Assert.Equal(businessTransaction.Fee, transaction.Fee);
            Assert.Equal(businessTransaction.TransactionDetails.IsConfirmed,
                transaction.TransactionDetails.IsConfirmed);
            Assert.Equal(businessTransaction.TransactionDetails.BlockId, transaction.TransactionDetails.BlockId);
            Assert.Equal(businessTransaction.TransactionDetails.BlocksBehind,
                transaction.TransactionDetails.BlocksBehind);
        }

        [Fact]
        public void AddTransaction_Transaction_Transaction()
        {
            // Arrange
            var transaction = new Models.Transaction {Id = "1", Amount = 10, Fee = 11};

            _transactionServiceMock.Setup(p => p.AddTransaction(It.IsAny<Transaction>()))
                .Returns((Transaction tr) => new SuccessResponse<Transaction>("Transaction has been added", tr));

            // Act
            var result = _transactionsController.AddTransaction(transaction);
            var response = result.Value;
            var resultTransaction = result.Value.Result as Models.Transaction;

            // Assert
            _transactionServiceMock.Verify(p => p.AddTransaction(It.IsAny<Transaction>()));

            Assert.NotNull(result);
            Assert.NotNull(response);
            Assert.NotEqual(Guid.Empty, response.Id);
            Assert.NotNull(response.Message);
            Assert.NotNull(resultTransaction);
            Assert.Equal(transaction.Id, resultTransaction.Id);
            Assert.Equal(transaction.Amount, resultTransaction.Amount);
            Assert.Equal(transaction.Fee, resultTransaction.Fee);
        }
    }
}