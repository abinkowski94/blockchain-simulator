using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Model.Block;
using BlockchainSimulator.Node.DataAccess.Repositories;
using BlockchainSimulator.Node.WebApi.Controllers;
using Moq;
using System.Collections.Generic;
using System.Linq;
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
            var blockchain = new BlockchainTree { Blocks = new List<BlockBase> { new Block { Id = id } } };

            _blockchainRepositoryMock.Setup(p => p.GetBlockchainTree())
                .Returns(blockchain);

            // Act
            var result = _blockchainController.GetBlock(id);
            var block = result.Value as Block;

            // Assert
            _blockchainRepositoryMock.Verify(p => p.GetBlockchainTree());

            Assert.NotNull(result);
            Assert.NotNull(block);
            Assert.Equal(blockchain.Blocks.First(), block);
            Assert.Equal(id, block.Id);
        }

        [Fact]
        public void GetBlock_Id_Null()
        {
            // Arrange
            const string id = "1";

            _blockchainRepositoryMock.Setup(p => p.GetBlockchainTree())
                .Returns((BlockchainTree)null);

            // Act
            var result = _blockchainController.GetBlock(id);
            var block = result.Value as Block;

            // Assert
            _blockchainRepositoryMock.Verify(p => p.GetBlockchainTree());

            Assert.NotNull(result);
            Assert.Null(block);
        }

        [Fact]
        public void GetBlockchain_Empty_Blockchain()
        {
            // Arrange
            _blockchainRepositoryMock.Setup(p => p.GetBlockchainTree())
                .Returns(new BlockchainTree());

            // Act
            var result = _blockchainController.GetBlockchain();

            // Assert
            _blockchainRepositoryMock.Verify(p => p.GetBlockchainTree());

            Assert.NotNull(result);
        }
    }
}