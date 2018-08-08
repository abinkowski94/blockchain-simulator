using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Model.Transaction;
using BlockchainSimulator.BusinessLogic.Providers;
using BlockchainSimulator.BusinessLogic.Services;
using BlockchainSimulator.BusinessLogic.Tests.Data;
using Xunit;

namespace BlockchainSimulator.BusinessLogic.Tests.Providers
{
    public class MerkleTreeProviderTests
    {
        private readonly MerkleTreeProvider _merkleTreeProvider;

        public MerkleTreeProviderTests()
        {
            _merkleTreeProvider = new MerkleTreeProvider(new EncryptionService());
        }

        [Fact]
        public void GetMerkleTree_Null_Null()
        {
            // Arrange

            // Act
            var result = _merkleTreeProvider.GetMerkleTree(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetMerkleTree_EmptyList_Null()
        {
            // Arrange
            var transactions = new HashSet<Transaction>();

            // Act
            var result = _merkleTreeProvider.GetMerkleTree(transactions);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [MemberData(nameof(MerkleTreeTransactionData.TransactionDataAndResults), MemberType = typeof(MerkleTreeTransactionData))]
        public void GetMerkleTree_SetOfTransactions_CorrectMerkleTree(HashSet<Transaction> transactions,
            string resultHash)
        {
            // Arrange

            // Act
            var result = _merkleTreeProvider.GetMerkleTree(transactions);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(resultHash, result.Hash);
        }
    }
}