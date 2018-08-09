using BlockchainSimulator.BusinessLogic.Providers;
using BlockchainSimulator.BusinessLogic.Services;
using BlockchainSimulator.BusinessLogic.Tests.Data;
using BlockchainSimulator.BusinessLogic.Validators;
using BlockchainSimulator.DataAccess.Model;
using BlockchainSimulator.DataAccess.Repositories;
using Moq;
using Xunit;

namespace BlockchainSimulator.BusinessLogic.Tests.Services
{
    public class BlockchainServiceTests
    {
        private readonly BlockchainService _blockchainService;
        private readonly Mock<IBlockchainValidator> _blockchainValidatorMock;
        private readonly Mock<IBlockProvider> _blockProviderMock;
        private readonly Mock<IBlockchainRepository> _blockchainRepositoryMock;

        public BlockchainServiceTests()
        {
            _blockchainValidatorMock = new Mock<IBlockchainValidator>();
            _blockProviderMock = new Mock<IBlockProvider>();
            _blockchainRepositoryMock = new Mock<IBlockchainRepository>();
            _blockchainService = new BlockchainService(_blockchainValidatorMock.Object, _blockProviderMock.Object,
                _blockchainRepositoryMock.Object);
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
    }
}