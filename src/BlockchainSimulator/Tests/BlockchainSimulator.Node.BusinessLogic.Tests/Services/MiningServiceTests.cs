using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Providers;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using BlockchainSimulator.Node.BusinessLogic.Services;
using BlockchainSimulator.Node.BusinessLogic.Tests.Data;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BlockchainSimulator.Node.BusinessLogic.Tests.Services
{
    public class MiningServiceTests
    {
        private readonly Mock<IBlockchainService> _blockchainServiceMock;
        private readonly Mock<IBlockProvider> _blockProviderMock;
        private readonly Mock<IConsensusService> _consensusServiceMock;
        private readonly Mock<IMiningQueue> _miningQueueMock;
        private readonly MiningService _miningService;

        public MiningServiceTests()
        {
            _blockchainServiceMock = new Mock<IBlockchainService>();
            _blockProviderMock = new Mock<IBlockProvider>();
            _consensusServiceMock = new Mock<IConsensusService>();
            _miningQueueMock = new Mock<IMiningQueue>();
            _miningService = new MiningService(_blockchainServiceMock.Object, _blockProviderMock.Object,
                _consensusServiceMock.Object, _miningQueueMock.Object, new Mock<IStatisticService>().Object);
        }

        [Fact]
        public void MineBlocks_TransactionsDateAndToken_Task()
        {
            // Arrange
            var transactions = TransactionDataSet.TransactionData
                .Select(ts => (HashSet<Transaction>)ts.First()).Last().ToList();
            var enqueueTime = new DateTime(1, 1, 1);
            var token = new CancellationToken();

            _blockchainServiceMock.Setup(p => p.GetBlockchain())
                .Returns(new SuccessResponse<BlockBase>("The blockchain", null));

            _blockProviderMock.Setup(p =>
                    p.CreateBlock(It.IsAny<HashSet<Transaction>>(), enqueueTime, It.IsAny<BlockBase>()))
                .Returns(new GenesisBlock());

            _consensusServiceMock.Setup(p => p.AcceptBlockchain(It.IsAny<BlockBase>()))
                .Returns(new SuccessResponse<bool>("The block has been accepted", true));

            // Act
            _miningService.MineBlock(transactions, enqueueTime, token);

            // Assert
            _blockchainServiceMock.Verify(p => p.GetBlockchain());
            _blockProviderMock.Verify(p =>
                p.CreateBlock(It.IsAny<HashSet<Transaction>>(), enqueueTime, It.IsAny<BlockBase>()));
            _consensusServiceMock.Verify(p => p.AcceptBlockchain(It.IsAny<BlockBase>()));
        }

        [Fact]
        public void MineBlocks_TransactionsDateAndToken_TaskAcceptFailed()
        {
            // Arrange
            var transactions = TransactionDataSet.TransactionData
                .Select(ts => (HashSet<Transaction>)ts.First()).Last().ToList();
            var enqueueTime = new DateTime(1, 1, 1);
            var token = new CancellationToken();

            _blockchainServiceMock.Setup(p => p.GetBlockchain())
                .Returns(new SuccessResponse<BlockBase>("The blockchain", null));

            _blockProviderMock.Setup(p =>
                    p.CreateBlock(It.IsAny<HashSet<Transaction>>(), enqueueTime, It.IsAny<BlockBase>()))
                .Returns(new GenesisBlock());

            _consensusServiceMock.Setup(p => p.AcceptBlockchain(It.IsAny<BlockBase>()))
                .Returns(new ErrorResponse<bool>("The block has not been accepted", false));

            // Act
            _miningService.MineBlock(transactions, enqueueTime, token);

            // Assert
            _blockchainServiceMock.Verify(p => p.GetBlockchain());
            _blockProviderMock.Verify(p =>
                p.CreateBlock(It.IsAny<HashSet<Transaction>>(), enqueueTime, It.IsAny<BlockBase>()));
            _consensusServiceMock.Verify(p => p.AcceptBlockchain(It.IsAny<BlockBase>()));
            _miningQueueMock.Verify(p => p.QueueMiningTask(It.IsAny<Func<CancellationToken, Task>>()));
        }
    }
}