using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.Responses;
using BlockchainSimulator.BusinessLogic.Model.Transaction;
using BlockchainSimulator.BusinessLogic.Providers;
using BlockchainSimulator.BusinessLogic.Services;
using BlockchainSimulator.BusinessLogic.Tests.Data;
using Moq;
using Xunit;

namespace BlockchainSimulator.BusinessLogic.Tests.Services
{
    public class MiningServiceTests
    {
        private readonly MiningService _miningService;
        private readonly Mock<IBlockchainService> _blockchainServiceMock;
        private readonly Mock<IBlockProvider> _blockProviderMock;
        private readonly Mock<IConsensusService> _consensusServiceMock;

        public MiningServiceTests()
        {
            _blockchainServiceMock = new Mock<IBlockchainService>();
            _blockProviderMock = new Mock<IBlockProvider>();
            _consensusServiceMock = new Mock<IConsensusService>();
            _miningService = new MiningService(_blockchainServiceMock.Object, _blockProviderMock.Object,
                _consensusServiceMock.Object);
        }

        [Fact]
        public async Task MineBlocks_TransactionsDateAndToken_Task()
        {
            // Arrange
            var transactions = TransactionDataSet.TransactionData
                .Select(ts => (HashSet<Transaction>) ts.First()).Last().ToList();
            var enqueueTime = new DateTime(1, 1, 1);
            var token = new CancellationToken();

            _blockchainServiceMock.Setup(p => p.GetBlockchain())
                .Returns(new SuccessResponse<BlockBase>("The blockchain", null));

            _blockProviderMock.Setup(p =>
                    p.CreateBlock(It.IsAny<HashSet<Transaction>>(), enqueueTime, It.IsAny<BlockBase>()))
                .Returns(new GenesisBlock());

            // Act
            await _miningService.MineBlocks(transactions, enqueueTime, token);

            // Assert
            _blockchainServiceMock.Verify(p => p.GetBlockchain());
            _blockProviderMock.Verify(p =>
                p.CreateBlock(It.IsAny<HashSet<Transaction>>(), enqueueTime, It.IsAny<BlockBase>()));
            _blockchainServiceMock.Verify(p => p.SaveBlockchain(It.IsAny<BlockBase>(), It.IsAny<List<BlockBase>>()));
            _consensusServiceMock.Verify(p => p.ReachConsensus());
        }
    }
}