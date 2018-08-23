using System;
using System.Collections.Generic;
using System.Linq;
using BlockchainSimulator.Node.BusinessLogic.Configurations;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Providers;
using BlockchainSimulator.Node.BusinessLogic.Providers.Specific;
using BlockchainSimulator.Node.BusinessLogic.Tests.Data;
using Moq;
using Xunit;

namespace BlockchainSimulator.Node.BusinessLogic.Tests.Providers.Specific
{
    public class ProofOfWorkBlockProviderTests
    {
        private readonly IBlockchainConfiguration _configuration;
        private readonly ProofOfWorkBlockProvider _blockProvider;

        public ProofOfWorkBlockProviderTests()
        {
            var configurationMock = new Mock<IBlockchainConfiguration>();
            configurationMock.Setup(p => p.Target).Returns("0000");
            configurationMock.Setup(p => p.Version).Returns("PoW-v1");
            configurationMock.Setup(p => p.BlockSize).Returns(10);
            _configuration = configurationMock.Object;
            
            _blockProvider = new ProofOfWorkBlockProvider(new MerkleTreeProvider(), _configuration);
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
        public void CreateBlock_EmptyTransactionSet_Null()
        {
            // Arrange

            // Act
            var result = _blockProvider.CreateBlock(new HashSet<Transaction>(), new DateTime(1, 1, 1));

            // Assert
            Assert.Null(result);
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
            Assert.Equal("da9e4ab040c871393429f7311263489d3ae62c70886a8c060fbce5ced276b064",
                result.Header.MerkleTreeRootHash);
            Assert.Equal(_configuration.Target, result.Header.Target);
            Assert.NotNull(result.Header.Nonce);
            Assert.Equal(transactions.Count, result.Body.TransactionCounter);
            Assert.NotNull(result.Body.MerkleTree);
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
            Assert.Equal("0", result.ParentId);
            Assert.Equal(_configuration.Version, result.Header.Version);
            Assert.NotNull(result.Header.ParentHash);
            Assert.Equal("da9e4ab040c871393429f7311263489d3ae62c70886a8c060fbce5ced276b064",
                result.Header.MerkleTreeRootHash);
            Assert.Equal(_configuration.Target, result.Header.Target);
            Assert.NotNull(result.Header.Nonce);
            Assert.Equal(transactions.Count, result.Body.TransactionCounter);
            Assert.NotNull(result.Body.MerkleTree);
        }
    }
}