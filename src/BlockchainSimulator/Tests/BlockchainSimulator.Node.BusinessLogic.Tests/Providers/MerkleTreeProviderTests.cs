using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Providers;
using System.Collections.Generic;
using Xunit;

namespace BlockchainSimulator.Node.BusinessLogic.Tests.Providers
{
    public class MerkleTreeProviderTests
    {
        private readonly MerkleTreeProvider _merkleTreeProvider;

        public MerkleTreeProviderTests()
        {
            _merkleTreeProvider = new MerkleTreeProvider();
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

        [Fact]
        public void GetMerkleTree_Null_Null()
        {
            // Arrange

            // Act
            var result = _merkleTreeProvider.GetMerkleTree(null);

            // Assert
            Assert.Null(result);
        }
    }
}