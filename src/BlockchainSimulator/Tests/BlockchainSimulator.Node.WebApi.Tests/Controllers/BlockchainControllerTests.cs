using BlockchainSimulator.Node.DataAccess.Model.Block;
using BlockchainSimulator.Node.DataAccess.Repositories;
using BlockchainSimulator.Node.WebApi.Controllers;
using Moq;
using Xunit;

namespace BlockchainSimulator.Node.WebApi.Tests.Controllers
{
    public class BlockchainControllerTests
    {
        private readonly BlockchainController _blockchainController;
        private readonly Mock<IBlockchainRepository> _blockchainRepositoryMock;

        public BlockchainControllerTests()
        {
            _blockchainRepositoryMock = new Mock<IBlockchainRepository>();
            _blockchainController = new BlockchainController(_blockchainRepositoryMock.Object);
        }

        [Fact]
        public void GetBlock_Id_Block()
        {
            // Arrange
            const string id = "1";
            var block = new Block {Id = id};

            _blockchainRepositoryMock.Setup(p => p.GetBlock(id))
                .Returns(block);

            // Act
            var result = _blockchainController.GetBlock(id);
            var blockResult = result.Value as Block;

            // Assert
            _blockchainRepositoryMock.Verify(p => p.GetBlock(id));

            Assert.NotNull(result);
            Assert.NotNull(blockResult);
            Assert.Equal(block.Id, blockResult.Id);
            Assert.Equal(block, blockResult);
        }

        [Fact]
        public void GetBlock_Id_Null()
        {
            // Arrange
            const string id = "1";

            _blockchainRepositoryMock.Setup(p => p.GetBlock(id))
                .Returns((BlockBase) null);

            // Act
            var result = _blockchainController.GetBlock(id);
            var blockResult = result.Value as Block;

            // Assert
            _blockchainRepositoryMock.Verify(p => p.GetBlock(id));

            Assert.NotNull(result);
            Assert.Null(blockResult);
        }
    }
}