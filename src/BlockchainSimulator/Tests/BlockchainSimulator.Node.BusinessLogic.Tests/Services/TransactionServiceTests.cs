using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlockchainSimulator.Node.BusinessLogic.Configurations;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Providers;
using BlockchainSimulator.Node.BusinessLogic.Providers.Specific;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using BlockchainSimulator.Node.BusinessLogic.Services;
using BlockchainSimulator.Node.BusinessLogic.Tests.Data;
using Moq;
using Xunit;

namespace BlockchainSimulator.Node.BusinessLogic.Tests.Services
{
    public class TransactionServiceTests
    {
        private readonly Mock<IBlockchainService> _blockchainServiceMock;
        private readonly Mock<IBlockchainConfiguration> _blockchainConfigurationMock;
        private readonly Mock<IMiningQueue> _miningQueueMock;
        private readonly TransactionService _transactionService;
        private readonly Mock<IMiningService> _miningServiceMock;

        public TransactionServiceTests()
        {
            _blockchainServiceMock = new Mock<IBlockchainService>();
            _blockchainConfigurationMock = new Mock<IBlockchainConfiguration>();
            _miningQueueMock = new Mock<IMiningQueue>();
            _miningServiceMock = new Mock<IMiningService>();

            _transactionService = new TransactionService(_blockchainServiceMock.Object, _miningServiceMock.Object,
                _blockchainConfigurationMock.Object, _miningQueueMock.Object);
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

            _blockchainConfigurationMock.Setup(p => p.BlockSize).Returns(10);


            // Act
            var result = _transactionService.AddTransaction(transaction) as SuccessResponse<Transaction>;

            // Assert
            _blockchainConfigurationMock.Verify(p => p.BlockSize);
            _miningQueueMock.Verify(p => p.QueueMiningTask(It.IsAny<Func<CancellationToken, Task>>()), Times.Never);

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

            _blockchainConfigurationMock.Setup(p => p.BlockSize).Returns(1);
            _miningQueueMock.Setup(p => p.QueueMiningTask(It.IsAny<Func<CancellationToken, Task>>()))
                .Callback((Func<CancellationToken, Task> func) => queueTask = func);

            // Act
            var result = _transactionService.AddTransaction(transaction) as SuccessResponse<Transaction>;
            var task = queueTask(token);
            task.Start();
            task.Wait(token);

            // Assert
            _blockchainConfigurationMock.Verify(p => p.BlockSize);
            _miningQueueMock.Verify(p => p.QueueMiningTask(It.IsAny<Func<CancellationToken, Task>>()));
            _miningServiceMock.Verify(p => p.MineBlocks(It.IsAny<IEnumerable<Transaction>>(), It.IsAny<DateTime>(),
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

            _blockchainConfigurationMock.Setup(p => p.BlockSize).Returns(10);
            _transactionService.AddTransaction(transactionOne);
            _transactionService.AddTransaction(transactionTwo);

            // Act
            var result = _transactionService.GetPendingTransactions() as SuccessResponse<List<Transaction>>;

            // Assert
            _blockchainConfigurationMock.Verify(p => p.BlockSize);

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
        public void GetTransaction_Id_ErrorResponseWhenReading()
        {
            // Arrange
            const string id = "17fea80a-efa4-4357-be00-a7e0c670ef53";

            _blockchainServiceMock.Setup(p => p.GetBlockchain())
                .Returns(new ErrorResponse<BlockBase>("File not found!", null));

            // Act
            var result = _transactionService.GetTransaction(id) as ErrorResponse<Transaction>;

            // Assert
            _blockchainServiceMock.Verify(p => p.GetBlockchain());

            Assert.NotNull(result);
            Assert.Null(result.Result);
            Assert.NotNull(result.Message);
            Assert.Single(result.Errors);
            Assert.Equal("File not found!", result.Errors.First());
            Assert.Equal("An error occured while reading blockchain from local storage!", result.Message);
        }

        [Fact]
        public void GetTransaction_Id_ErrorResponseNoTransaction()
        {
            // Arrange
            const string id = "17fea80a-efa4-4357-be00-a7e0c670ef53";

            _blockchainServiceMock.Setup(p => p.GetBlockchain())
                .Returns(new SuccessResponse<BlockBase>("The blockchain!", null));

            // Act
            var result = _transactionService.GetTransaction(id) as ErrorResponse<Transaction>;

            // Assert
            _blockchainServiceMock.Verify(p => p.GetBlockchain());

            Assert.NotNull(result);
            Assert.Null(result.Result);
            Assert.NotNull(result.Message);
            Assert.Empty(result.Errors);
            Assert.Equal($"Could not find the transaction id: {id}", result.Message);
        }

        [Fact]
        public void GetTransaction_Id_SuccessResponse()
        {
            // Arrange
            _blockchainConfigurationMock.Setup(p => p.BlockSize).Returns(10);
            _blockchainConfigurationMock.Setup(p => p.Target).Returns("0000");
            _blockchainConfigurationMock.Setup(p => p.Version).Returns("PoW-v1");

            var blockchainProvider =
                new ProofOfWorkBlockProvider(new MerkleTreeProvider(), _blockchainConfigurationMock.Object);

            var transactionSetList = TransactionDataSet.TransactionData.Select(ts => (HashSet<Transaction>) ts.First())
                .ToList();

            transactionSetList[1].First().Id = "111111";

            BlockBase block = null;
            transactionSetList.ForEach(ts => block = blockchainProvider.CreateBlock(ts, new DateTime(1, 1, 1), block));

            _blockchainServiceMock.Setup(p => p.GetBlockchain())
                .Returns(new SuccessResponse<BlockBase>("The blockchain!", block));

            const string id = "111111";

            // Act
            var result = _transactionService.GetTransaction(id) as SuccessResponse<Transaction>;

            // Assert
            _blockchainServiceMock.Verify(p => p.GetBlockchain());

            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Equal(id, result.Result.Id);
            Assert.NotNull(result.Message);
            Assert.Equal("The transaction has been found", result.Message);
        }
    }
}