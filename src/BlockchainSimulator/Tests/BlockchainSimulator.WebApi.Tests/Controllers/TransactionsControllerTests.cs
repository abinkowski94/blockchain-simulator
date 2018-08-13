using System.Collections.Generic;
using System.Linq;
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
                .Returns(businessTransaction);

            // Act
            var result = _transactionsController.GetTransaction(id);
            var transaction = result.Value;

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
                .Returns(new List<Transaction> {businessTransaction, businessTransaction});

            // Act
            var result = _transactionsController.GetPendingTransactions();
            var transactions = result.Value;
            var transaction = transactions.First();

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
    }
}