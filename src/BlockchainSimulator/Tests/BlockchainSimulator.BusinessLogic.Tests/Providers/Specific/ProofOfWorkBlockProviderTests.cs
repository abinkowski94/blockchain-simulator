using System.Collections.Generic;
using System.Linq;
using BlockchainSimulator.BusinessLogic.Configurations;
using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.Transaction;
using BlockchainSimulator.BusinessLogic.Providers;
using BlockchainSimulator.BusinessLogic.Providers.Specific;
using BlockchainSimulator.BusinessLogic.Tests.Data;
using Xunit;

namespace BlockchainSimulator.BusinessLogic.Tests.Providers.Specific
{
    public class ProofOfWorkBlockProviderTests
    {
        private readonly ProofOfWorkBlockProvider _blockProvider;

        public ProofOfWorkBlockProviderTests()
        {
            _blockProvider = new ProofOfWorkBlockProvider(new MerkleTreeProvider());
        }

        [Fact]
        public void CreateBlock_Null_Null()
        {
            // Arrange

            // Act
            var result = _blockProvider.CreateBlock(null);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public void CreateBlock_EmptyTransactionSet_Null()
        {
            // Arrange

            // Act
            var result = _blockProvider.CreateBlock(new HashSet<Transaction>());

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public void CreateBlock_Transactions_GenesisBlock()
        {
            // Arrange
            var transactions = (HashSet<Transaction>) TransactionDataSet.TransactionData.Last().First();

            // Act
            var result = _blockProvider.CreateBlock(transactions);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsGenesis);
            Assert.Equal("0", result.Id);
            Assert.Equal(ProofOfWorkConfigurations.Version, result.Header.Version);
            Assert.Null(result.Header.ParentHash);
            Assert.Equal("527f2414cd36a489c11d018f71dad8ba609ae1a7781a103c1ba5bf249ac5de87",
                result.Header.MerkleTreeRootHash);
            Assert.Equal(ProofOfWorkConfigurations.Target, result.Header.Target);
            Assert.NotNull(result.Header.Nonce);
            Assert.Equal(transactions.Count, result.Body.TransactionCounter);
            Assert.NotNull(result.Body.MerkleTree);
        }

        [Fact]
        public void CreateBlock_Transactions_Block()
        {
            // Arrange
            var parentTransactions = (HashSet<Transaction>) TransactionDataSet.TransactionData.First().First();
            var parent = _blockProvider.CreateBlock(parentTransactions);
            var transactions = (HashSet<Transaction>) TransactionDataSet.TransactionData.Last().First();

            // Act
            var result = _blockProvider.CreateBlock(transactions, parent) as Block;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Parent);
            Assert.False(result.IsGenesis);
            Assert.Equal("1", result.Id);
            Assert.Equal("0", result.ParentId);
            Assert.Equal(ProofOfWorkConfigurations.Version, result.Header.Version);
            Assert.NotNull(result.Header.ParentHash);
            Assert.Equal("527f2414cd36a489c11d018f71dad8ba609ae1a7781a103c1ba5bf249ac5de87",
                result.Header.MerkleTreeRootHash);
            Assert.Equal(ProofOfWorkConfigurations.Target, result.Header.Target);
            Assert.NotNull(result.Header.Nonce);
            Assert.Equal(transactions.Count, result.Body.TransactionCounter);
            Assert.NotNull(result.Body.MerkleTree);
        }
    }
}