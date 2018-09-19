using BlockchainSimulator.Node.BusinessLogic.Configurations;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Providers;
using BlockchainSimulator.Node.BusinessLogic.Providers.Specific;
using BlockchainSimulator.Node.BusinessLogic.Tests.Data;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BlockchainSimulator.Node.BusinessLogic.Tests.Providers.Specific
{
    public class ProofOfWorkBlockProviderTests
    {
        private readonly ProofOfWorkBlockProvider _blockProvider;
        private readonly IBlockchainConfiguration _configuration;

        public ProofOfWorkBlockProviderTests()
        {
            var configurationMock = new Mock<IBlockchainConfiguration>();
            configurationMock.Setup(p => p.Target).Returns("0000");
            configurationMock.Setup(p => p.Version).Returns("PoW-v1");
            configurationMock.Setup(p => p.BlockSize).Returns(10);
            _configuration = configurationMock.Object;

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(p => p["Node:Id"]).Returns("1");

            _blockProvider = new ProofOfWorkBlockProvider(new MerkleTreeProvider(), _configuration, configMock.Object);
        }

        [Fact]
        public void CreateBlock_EmptyTransactionSet_Null()
        {
            // Arrange

            // Act
            var result = _blockProvider.CreateBlock(new HashSet<Transaction>(), new DateTime(1, 1, 1));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CreateBlock_Null_Null()
        {
            // Arrange

            // Act
            var result = _blockProvider.CreateBlock(null, new DateTime(1, 1, 1));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CreateBlock_Transactions_Block()
        {
            // Arrange
            var parentTransactions = (HashSet<Transaction>) TransactionDataSet.TransactionData.First().First();
            var parent = _blockProvider.CreateBlock(parentTransactions, new DateTime(1, 1, 1));
            var transactions = (HashSet<Transaction>) TransactionDataSet.TransactionData.Last().First();

            // Act
            var result = _blockProvider.CreateBlock(transactions, new DateTime(1, 1, 1), parent) as Block;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Parent);
            Assert.False(result.IsGenesis);
            Assert.Equal("1", result.Id);
            Assert.NotNull(result.UniqueId);
            Assert.Equal(result.Parent.UniqueId, result.ParentUniqueId);
            Assert.Equal(_configuration.Version, result.Header.Version);
            Assert.NotNull(result.Header.ParentHash);
            Assert.Equal("5e2da3deb36395f11e6a1115ecda4973ca94424881e104267808f0bc79c1c58f",
                result.Header.MerkleTreeRootHash);
            Assert.Equal(_configuration.Target, result.Header.Target);
            Assert.NotNull(result.Header.Nonce);
            Assert.Equal(transactions.Count, result.Body.TransactionCounter);
            Assert.NotNull(result.Body.MerkleTree);
        }

        [Fact]
        public void CreateBlock_Transactions_GenesisBlock()
        {
            // Arrange
            var transactions = (HashSet<Transaction>) TransactionDataSet.TransactionData.Last().First();

            // Act
            var result = _blockProvider.CreateBlock(transactions, new DateTime(1, 1, 1));

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsGenesis);
            Assert.Equal("0", result.Id);
            Assert.Equal(_configuration.Version, result.Header.Version);
            Assert.Null(result.Header.ParentHash);
            Assert.Equal("5e2da3deb36395f11e6a1115ecda4973ca94424881e104267808f0bc79c1c58f",
                result.Header.MerkleTreeRootHash);
            Assert.Equal(_configuration.Target, result.Header.Target);
            Assert.NotNull(result.Header.Nonce);
            Assert.Equal(transactions.Count, result.Body.TransactionCounter);
            Assert.NotNull(result.Body.MerkleTree);
        }
    }
}