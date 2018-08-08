using System.Collections.Generic;
using System.Linq;
using BlockchainSimulator.BusinessLogic.Model.Transaction;
using BlockchainSimulator.BusinessLogic.Providers;
using BlockchainSimulator.BusinessLogic.Services;
using BlockchainSimulator.BusinessLogic.Tests.Data;
using BlockchainSimulator.BusinessLogic.Validators;
using Xunit;

namespace BlockchainSimulator.BusinessLogic.Tests.Validators
{
    public class MerkleTreeValidatorTests
    {
        private readonly MerkleTreeValidator _merkleTreeValidator;

        public MerkleTreeValidatorTests()
        {
            _merkleTreeValidator = new MerkleTreeValidator(new EncryptionService());
        }

        [Fact]
        public void Validate_Null_SuccessValidationResult()
        {
            // Arrange

            // Act
            var result = _merkleTreeValidator.Validate(null);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Errors);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_WrongTreeHash_ErrorValidationResult()
        {
            // Arrange
            var tree = new MerkleTreeProvider(new EncryptionService())
                .GetMerkleTree(MerkleTreeTransactionData.TransactionData.Last().First() as HashSet<Transaction>);
            ((Node) tree.LeftNode).LeftNode.Hash = "000";

            // Act
            var result = _merkleTreeValidator.Validate(tree);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
            Assert.NotEmpty(result.Errors);
            Assert.Equal(
                "Wrong hash for nodes with hashes h1:000 and h2: 21340e8e71a47b4a0bb7759db7a7768e53ebf33e097e8e9d4bddbbe497f62516",
                result.Errors.First());
        }

        [Theory]
        [MemberData(nameof(MerkleTreeTransactionData.TransactionData), MemberType = typeof(MerkleTreeTransactionData))]
        public void Validate_WrongTree_ErrorValidationResult(HashSet<Transaction> transactions)
        {
            // Arrange
            var tree = new MerkleTreeProvider(new EncryptionService()).GetMerkleTree(transactions);
            tree.Hash = "00000000000000";

            // Act
            var result = _merkleTreeValidator.Validate(tree);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
            Assert.NotEmpty(result.Errors);
        }

        [Theory]
        [MemberData(nameof(MerkleTreeTransactionData.TransactionData), MemberType = typeof(MerkleTreeTransactionData))]
        public void Validate_CorrectTree_SuccessValidationResult(HashSet<Transaction> transactions)
        {
            // Arrange
            var tree = new MerkleTreeProvider(new EncryptionService()).GetMerkleTree(transactions);

            // Act
            var result = _merkleTreeValidator.Validate(tree);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Errors);
            Assert.Empty(result.Errors);
        }
    }
}