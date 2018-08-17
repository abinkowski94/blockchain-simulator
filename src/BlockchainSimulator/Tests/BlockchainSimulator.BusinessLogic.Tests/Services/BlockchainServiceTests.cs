using System;
using System.Collections.Generic;
using System.Linq;
using BlockchainSimulator.BusinessLogic.Configurations;
using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.Transaction;
using BlockchainSimulator.BusinessLogic.Providers;
using BlockchainSimulator.BusinessLogic.Providers.Specific;
using BlockchainSimulator.BusinessLogic.Services;
using BlockchainSimulator.BusinessLogic.Tests.Data;
using BlockchainSimulator.DataAccess.Repositories;
using Moq;
using Xunit;

namespace BlockchainSimulator.BusinessLogic.Tests.Services
{
    public class BlockchainServiceTests
    {
        private readonly IBlockchainConfiguration _configuration;
        private readonly BlockchainService _blockchainService;
        private readonly Mock<IBlockchainRepository> _blockchainRepositoryMock;

        public BlockchainServiceTests()
        {
            var configurationMock = new Mock<IBlockchainConfiguration>();
            configurationMock.Setup(p => p.Target).Returns("0000");
            configurationMock.Setup(p => p.Version).Returns("PoW-v1");
            configurationMock.Setup(p => p.BlockSize).Returns(10);
            _configuration = configurationMock.Object;

            _blockchainRepositoryMock = new Mock<IBlockchainRepository>();
            _blockchainService = new BlockchainService(_blockchainRepositoryMock.Object);
        }

        [Theory]
        [MemberData(nameof(BlockchainDataSet.BlockchainData), MemberType = typeof(BlockchainDataSet))]
        public void GetBlockchain_NoParams_Blockchain(DataAccess.Model.Blockchain blockchain)
        {
            // Arrange
            _blockchainRepositoryMock.Setup(p => p.GetBlockchain())
                .Returns(blockchain);

            // Act
            var result = _blockchainService.GetBlockchain();

            // Assert
            _blockchainRepositoryMock.Verify(p => p.GetBlockchain());

            Assert.NotNull(result);
        }

        [Fact]
        public void SaveBlockchain_Null_Void()
        {
            // Arrange

            // Act
            _blockchainService.SaveBlockchain(null);

            // Assert
            _blockchainRepositoryMock.Verify(p => p.SaveBlockchain(It.IsAny<DataAccess.Model.Blockchain>()),
                Times.Never);
        }

        [Fact]
        public void SaveBlockchain_Blocks_Void()
        {
            // Arrange
            var blockchainProvider = new ProofOfWorkBlockProvider(new MerkleTreeProvider(), _configuration);

            var transactionSetList = TransactionDataSet.TransactionData.Select(ts => (HashSet<Transaction>) ts.First())
                .ToList();

            BlockBase block = null;
            transactionSetList.ForEach(ts => block = blockchainProvider.CreateBlock(ts, new DateTime(1, 1, 1), block));

            // Act
            _blockchainService.SaveBlockchain(block);

            // Assert
            _blockchainRepositoryMock.Verify(p => p.SaveBlockchain(It.IsAny<DataAccess.Model.Blockchain>()));
        }
    }
}