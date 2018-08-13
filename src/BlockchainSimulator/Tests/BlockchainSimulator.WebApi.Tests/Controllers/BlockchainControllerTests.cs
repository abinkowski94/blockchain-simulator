using BlockchainSimulator.DataAccess.Model;
using BlockchainSimulator.DataAccess.Repositories;
using BlockchainSimulator.WebApi.Controllers;
using Moq;
using Xunit;

namespace BlockchainSimulator.WebApi.Tests.Controllers
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
        public void GetBlockchain_Empty_Blockchain()
        {
            // Arrange
            _blockchainRepositoryMock.Setup(p => p.GetBlockchain())
                .Returns(new Blockchain());

            // Act
            var result = _blockchainController.GetBlockchain();

            // Assert
            _blockchainRepositoryMock.Verify(p => p.GetBlockchain());
            
            Assert.NotNull(result);
        }
     }
 }