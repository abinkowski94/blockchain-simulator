using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Services;
using BlockchainSimulator.Node.BusinessLogic.Services.Specific;
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
        private readonly ProofOfWorkBlockchainService _proofOfWorkBlockchainService;

        public BlockchainServiceTests()
        {
            _blockchainRepositoryMock = new Mock<IBlockchainRepository>();
            _proofOfWorkBlockchainService = new ProofOfWorkBlockchainService(new Mock<IConfigurationService>().Object,
                _blockchainRepositoryMock.Object);
        }

        [Theory]
        [MemberData(nameof(BlockchainDataSet.BlockchainData), MemberType = typeof(BlockchainDataSet))]
        public void GetBlockchain_NoParams_Blockchain(BlockchainTree blockchainTree)
        {
            // Arrange
            _blockchainRepositoryMock.Setup(p => p.GetBlockchainTree())
                .Returns(blockchainTree);

            // Act
            var result = _proofOfWorkBlockchainService.GetBlockchainTree();

            // Assert
            _blockchainRepositoryMock.Verify(p => p.GetBlockchainTree());

            Assert.NotNull(result);
        }

        [Fact]
        public void GetBlockchainTreeLinked_NoParams_ErrorResponseNoBlocks()
        {
            // Arrange
            _blockchainRepositoryMock.Setup(p => p.GetBlockchainTree())
                .Returns(new BlockchainTree());

            // Act
            var result = _proofOfWorkBlockchainService.GetBlockchainTreeLinked() as ErrorResponse<BlockBase>;

            // Assert
            _blockchainRepositoryMock.Verify(p => p.GetBlockchainTree());

            Assert.NotNull(result);
            Assert.NotNull(result.Message);
            Assert.Null(result.Result);
            Assert.Equal("The blockchain tree does not contain blocks!", result.Message);
        }
    }
}