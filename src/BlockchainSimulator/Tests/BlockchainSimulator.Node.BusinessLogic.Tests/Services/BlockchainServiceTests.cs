using BlockchainSimulator.Node.BusinessLogic.Configurations;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Services;
using BlockchainSimulator.Node.BusinessLogic.Tests.Data;
using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Repositories;
using Moq;
using Xunit;

namespace BlockchainSimulator.Node.BusinessLogic.Tests.Services
{
    public class BlockchainServiceTests
    {
        private readonly Mock<IBlockchainRepository> _blockchainRepositoryMock;
        private readonly BlockchainService _blockchainService;

        public BlockchainServiceTests()
        {
            var configurationMock = new Mock<IBlockchainConfiguration>();
            configurationMock.Setup(p => p.Target).Returns("0000");
            configurationMock.Setup(p => p.Version).Returns("PoW-v1");
            configurationMock.Setup(p => p.BlockSize).Returns(10);

            _blockchainRepositoryMock = new Mock<IBlockchainRepository>();
            _blockchainService = new BlockchainService(_blockchainRepositoryMock.Object);
        }

        [Theory]
        [MemberData(nameof(BlockchainDataSet.BlockchainData), MemberType = typeof(BlockchainDataSet))]
        public void GetBlockchain_NoParams_Blockchain(Blockchain blockchain)
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
        public void GetBlockchain_NoParams_ErrorResponseNoBlocks()
        {
            // Arrange
            _blockchainRepositoryMock.Setup(p => p.GetBlockchain())
                .Returns(new Blockchain());

            // Act
            var result = _blockchainService.GetBlockchain() as ErrorResponse<BlockBase>;

            // Assert
            _blockchainRepositoryMock.Verify(p => p.GetBlockchain());

            Assert.NotNull(result);
            Assert.NotNull(result.Message);
            Assert.Null(result.Result);
            Assert.Equal("The blockchain does not contain blocks", result.Message);
        }
    }
}