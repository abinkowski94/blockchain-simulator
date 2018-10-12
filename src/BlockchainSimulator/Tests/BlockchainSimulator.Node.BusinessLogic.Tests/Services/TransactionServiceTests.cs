using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Common.Models;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Providers;
using BlockchainSimulator.Node.BusinessLogic.Providers.Specific;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using BlockchainSimulator.Node.BusinessLogic.Services;
using BlockchainSimulator.Node.BusinessLogic.Tests.Data;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlockchainSimulator.Node.BusinessLogic.Storage;
using Xunit;

namespace BlockchainSimulator.Node.BusinessLogic.Tests.Services
{
    public class TransactionServiceTests
    {
        private readonly Mock<IConfigurationService> _configurationServiceMock;
        private readonly Mock<IBlockchainService> _blockchainServiceMock;
        private readonly Mock<IMiningQueue> _miningQueueMock;
        private readonly Mock<IMiningService> _miningServiceMock;
        private readonly TransactionService _transactionService;

        public TransactionServiceTests()
        {
            _configurationServiceMock = new Mock<IConfigurationService>();
            _blockchainServiceMock = new Mock<IBlockchainService>();
            _miningQueueMock = new Mock<IMiningQueue>();
            _miningServiceMock = new Mock<IMiningService>();

            _configurationServiceMock.Setup(p => p.GetConfiguration())
                .Returns(new BlockchainNodeConfiguration
                {
                    Target = "0000",
                    Version = "PoW-v1",
                    BlockSize = 10,
                    NodeId = "1"
                });

            _transactionService = new TransactionService(_configurationServiceMock.Object,
                _blockchainServiceMock.Object, _miningServiceMock.Object, _miningQueueMock.Object,
                new TransactionStorage());
        }

        [Fact]
        public void AddTransaction_Transaction_SuccessResponseWithTransaction()
        {
            // Arrange
            var transaction = new Transaction
            {
                Sender = "4c9395c5-07d9-42c2-9fe9-19478b312acf",
                Recipient = "75f05331-a972-4fa5-b095-0672a8f2c537",
                Amount = 100,
                Fee = 2
            };

            // Act
            var result = _transactionService.AddTransaction(transaction) as SuccessResponse<Transaction>;

            // Assert
            _miningQueueMock.Verify(p => p.EnqueueTask(It.IsAny<Func<CancellationToken, Task>>()), Times.Never);

            Assert.NotNull(result);
            Assert.NotNull(result.Message);
            Assert.NotNull(result.Result);
            Assert.NotNull(result.Result.Id);
            Assert.Equal(transaction, result.Result);
            Assert.Equal("The transaction has been added to pending list", result.Message);
        }

        [Fact]
        public void AddTransaction_Transaction_SuccessResponseWithTransactionMiningStarted()
        {
            // Arrange
            var transaction = new Transaction
            {
                Sender = "4c9395c5-07d9-42c2-9fe9-19478b312acf",
                Recipient = "75f05331-a972-4fa5-b095-0672a8f2c537",
                Amount = 100,
                Fee = 2
            };

            var token = new CancellationToken();
            Func<CancellationToken, Task> queueTask = t => Task.Run(() => { }, t);

            _miningQueueMock.Setup(p => p.EnqueueTask(It.IsAny<Func<CancellationToken, Task>>()))
                .Callback((Func<CancellationToken, Task> func) => queueTask = func);

            LinqExtensions.RepeatAction(9, () => _transactionService.AddTransaction(new Transaction
            {
                Sender = "4c9395c5-07d9-42c2-9fe9-19478b312acf",
                Recipient = "75f05331-a972-4fa5-b095-0672a8f2c537",
                Amount = 100,
                Fee = 2
            }));

            // Act
            var result = _transactionService.AddTransaction(transaction) as SuccessResponse<Transaction>;
            var task = queueTask(token);
            task.Start();
            task.Wait(token);

            // Assert
            _miningQueueMock.Verify(p => p.EnqueueTask(It.IsAny<Func<CancellationToken, Task>>()));
            _miningServiceMock.Verify(p => p.MineBlock(It.IsAny<HashSet<Transaction>>(), It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()));

            Assert.NotNull(result);
            Assert.NotNull(result.Message);
            Assert.NotNull(result.Result);
            Assert.NotNull(result.Result.Id);
            Assert.Equal(transaction, result.Result);
            Assert.Equal("The transaction has been added and processing has started", result.Message);
        }

        [Fact]
        public void GetPendingTransactions_Empty_SuccessResponseWithListOfTransactions()
        {
            // Arrange
            var transactionOne = new Transaction
            {
                Sender = "4c9395c5-07d9-42c2-9fe9-19478b312acf",
                Recipient = "75f05331-a972-4fa5-b095-0672a8f2c537",
                Amount = 100,
                Fee = 2
            };

            var transactionTwo = new Transaction
            {
                Sender = "75f05331-a972-4fa5-b095-0672a8f2c537",
                Recipient = "4c9395c5-07d9-42c2-9fe9-19478b312acf",
                Amount = 21,
                Fee = 1
            };

            _transactionService.AddTransaction(transactionOne);
            _transactionService.AddTransaction(transactionTwo);

            // Act
            var result = _transactionService.GetPendingTransactions() as SuccessResponse<List<Transaction>>;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Message);
            Assert.NotNull(result.Result);
            Assert.NotEmpty(result.Result);
            Assert.Equal(2, result.Result.Count);
            Assert.Equal(transactionOne, result.Result.First());
            Assert.Equal(transactionTwo, result.Result.Last());
            Assert.Equal("Pending transactions count: 2/10", result.Message);
        }

        [Fact]
        public void GetTransaction_Id_ErrorResponseNoTransaction()
        {
            // Arrange
            const string id = "17fea80a-efa4-4357-be00-a7e0c670ef53";

            _blockchainServiceMock.Setup(p => p.GetBlockchainTree())
                .Returns(new SuccessResponse<BlockBase>("The blockchain!", null));

            // Act
            var result = _transactionService.GetTransaction(id) as ErrorResponse<Transaction>;

            // Assert
            _blockchainServiceMock.Verify(p => p.GetBlockchainTree());

            Assert.NotNull(result);
            Assert.Null(result.Result);
            Assert.NotNull(result.Message);
            Assert.Empty(result.Errors);
            Assert.Equal($"Could not find the transaction id: {id}", result.Message);
        }

        [Fact]
        public void GetTransaction_Id_ErrorResponseWhenReading()
        {
            // Arrange
            const string id = "17fea80a-efa4-4357-be00-a7e0c670ef53";

            _blockchainServiceMock.Setup(p => p.GetBlockchainTree())
                .Returns(new ErrorResponse<BlockBase>("File not found!", null));

            // Act
            var result = _transactionService.GetTransaction(id) as ErrorResponse<Transaction>;

            // Assert
            _blockchainServiceMock.Verify(p => p.GetBlockchainTree());

            Assert.NotNull(result);
            Assert.Null(result.Result);
            Assert.NotNull(result.Message);
            Assert.Single(result.Errors);
            Assert.Equal("File not found!", result.Errors.First());
            Assert.Equal("An error occurred while reading blockchain from local storage!", result.Message);
        }

        [Fact]
        public void GetTransaction_Id_SuccessResponse()
        {
            // Arrange
            var blockchainProvider =
                new ProofOfWorkBlockProvider(new MerkleTreeProvider(), _configurationServiceMock.Object);

            var transactionSetList = TransactionDataSet.TransactionData.Select(ts => (HashSet<Transaction>) ts.First())
                .ToList();

            transactionSetList[1].First().Id = "111111";

            BlockBase block = null;
            transactionSetList.ForEach(ts =>
                block = blockchainProvider.CreateBlock(ts, new DateTime(1, 1, 1), block).Result);

            _blockchainServiceMock.Setup(p => p.GetBlockchainTree())
                .Returns(new SuccessResponse<BlockBase>("The blockchain!", block));

            const string id = "111111";

            // Act
            var result = _transactionService.GetTransaction(id) as SuccessResponse<Transaction>;

            // Assert
            _blockchainServiceMock.Verify(p => p.GetBlockchainTree());

            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Equal(id, result.Result.Id);
            Assert.NotNull(result.Message);
            Assert.Equal("The transaction has been found", result.Message);
        }
    }
}