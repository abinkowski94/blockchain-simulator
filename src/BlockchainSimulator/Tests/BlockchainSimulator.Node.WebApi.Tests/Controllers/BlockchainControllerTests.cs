using BlockchainSimulator.Node.BusinessLogic.Services;
using BlockchainSimulator.Node.DataAccess.Model.Block;
using BlockchainSimulator.Node.WebApi.Controllers;
using Moq;
using Xunit;

namespace BlockchainSimulator.Node.WebApi.Tests.Controllers
{
    public class BlockchainControllerTests
    {
        private readonly BlockchainController _blockchainController;
        private readonly Mock<IBlockchainService> _blockchainServiceMock;

        public BlockchainControllerTests()
        {
            _blockchainServiceMock = new Mock<IBlockchainService>();
            _blockchainController = new BlockchainController(_blockchainServiceMock.Object);
        }

        [Fact]
        public void GetBlock_Id_Block()
        {
            // Arrange
            const string id = "1";
            var block = new Block { Id = id };

            _blockchainServiceMock.Setup(p => p.GetBlock(id))
                .Returns(block);

            // Act
            var result = _blockchainController.GetBlock(id);
            var blockResult = result.Value as Block;

            // Assert
            _blockchainServiceMock.Verify(p => p.GetBlock(id));

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

            _blockchainServiceMock.Setup(p => p.GetBlock(id))
                .Returns((BlockBase)null);

            // Act
            var result = _blockchainController.GetBlock(id);
            var blockResult = result.Value as Block;

            // Assert
            _blockchainServiceMock.Verify(p => p.GetBlock(id));

            Assert.NotNull(result);
            Assert.Null(blockResult);
        }
    }
}