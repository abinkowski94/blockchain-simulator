using BlockchainSimulator.Common.Models;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Providers;
using BlockchainSimulator.Node.BusinessLogic.Providers.Specific;
using BlockchainSimulator.Node.BusinessLogic.Services;
using BlockchainSimulator.Node.BusinessLogic.Tests.Data;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BlockchainSimulator.Node.BusinessLogic.Tests.Providers.Specific
{
    public class ProofOfWorkBlockProviderTests
    {
        private readonly BlockchainNodeConfiguration _blockchainNodeConfiguration;
        private readonly Mock<IConfigurationService> _configurationServiceMock;
        private readonly ProofOfWorkBlockProvider _blockProvider;

        public ProofOfWorkBlockProviderTests()
        {
            _blockchainNodeConfiguration = new BlockchainNodeConfiguration
            {
                Target = "0000",
                Version = "PoW-v1",
                BlockSize = 10,
                NodeId = "1"
            };
            _configurationServiceMock = new Mock<IConfigurationService>();
            _configurationServiceMock.Setup(p => p.GetConfiguration())
                .Returns(_blockchainNodeConfiguration);

            _blockProvider = new ProofOfWorkBlockProvider(new MerkleTreeProvider(), _configurationServiceMock.Object);
        }

        [Fact]
        public async Task CreateBlock_EmptyTransactionSet_Null()
        {
            // Arrange

            // Act
            var result = await _blockProvider.CreateBlock(new HashSet<Transaction>(), new DateTime(1, 1, 1));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateBlock_Null_Null()
        {
            // Arrange

            // Act
            var result = await _blockProvider.CreateBlock(null, new DateTime(1, 1, 1));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateBlock_Transactions_Block()
        {
            // Arrange
            var parentTransactions = (HashSet<Transaction>)TransactionDataSet.TransactionData.First().First();
            var parent = await _blockProvider.CreateBlock(parentTransactions, new DateTime(1, 1, 1));
            var transactions = (HashSet<Transaction>)TransactionDataSet.TransactionData.Last().First();

            // Act
            var result = await _blockProvider.CreateBlock(transactions, new DateTime(1, 1, 1), parent) as Block;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Parent);
            Assert.False(result.IsGenesis);
            Assert.Equal("1", result.Id);
            Assert.NotNull(result.UniqueId);
            Assert.Equal(result.Parent.UniqueId, result.ParentUniqueId);
            Assert.Equal(_blockchainNodeConfiguration.Version, result.Header.Version);
            Assert.NotNull(result.Header.ParentHash);
            Assert.Equal("5e2da3deb36395f11e6a1115ecda4973ca94424881e104267808f0bc79c1c58f",
                result.Header.MerkleTreeRootHash);
            Assert.Equal(_blockchainNodeConfiguration.Target, result.Header.Target);
            Assert.NotNull(result.Header.Nonce);
            Assert.Equal(transactions.Count, result.Body.TransactionCounter);
            Assert.NotNull(result.Body.MerkleTree);
        }

        [Fact]
        public async Task CreateBlock_Transactions_GenesisBlock()
        {
            // Arrange
            var transactions = (HashSet<Transaction>)TransactionDataSet.TransactionData.Last().First();

            // Act
            var result = await _blockProvider.CreateBlock(transactions, new DateTime(1, 1, 1));

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsGenesis);
            Assert.Equal("0", result.Id);
            Assert.Equal(_blockchainNodeConfiguration.Version, result.Header.Version);
            Assert.Null(result.Header.ParentHash);
            Assert.Equal("5e2da3deb36395f11e6a1115ecda4973ca94424881e104267808f0bc79c1c58f",
                result.Header.MerkleTreeRootHash);
            Assert.Equal(_blockchainNodeConfiguration.Target, result.Header.Target);
            Assert.NotNull(result.Header.Nonce);
            Assert.Equal(transactions.Count, result.Body.TransactionCounter);
            Assert.NotNull(result.Body.MerkleTree);
        }
    }
}