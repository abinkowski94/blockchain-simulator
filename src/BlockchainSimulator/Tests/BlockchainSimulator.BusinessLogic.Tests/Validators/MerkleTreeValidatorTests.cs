using System.Collections.Generic;
using System.Linq;
using BlockchainSimulator.BusinessLogic.Model.Transaction;
using BlockchainSimulator.BusinessLogic.Providers;
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
            _merkleTreeValidator = new MerkleTreeValidator();
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
            var tree = new MerkleTreeProvider().GetMerkleTree(
                TransactionDataSet.TransactionData.Last().First() as HashSet<Transaction>);
            ((Node) tree.LeftNode).LeftNode.Hash = "000";

            // Act
            var result = _merkleTreeValidator.Validate(tree);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
            Assert.NotEmpty(result.Errors);
            Assert.Equal(
                "Wrong hash for nodes with hashes h1:000 and h2: 05615a4f5e18817f431e536f40a36b18ab105ddb94ddafa8aaaafb86cdd51e06",
                result.Errors.First());
        }

        [Fact]
        public void Validate_WrongLeafHash_ErrorValidationResult()
        {
            // Arrange
            var tree = new MerkleTreeProvider().GetMerkleTree(
                TransactionDataSet.TransactionData.First().First() as HashSet<Transaction>);
            tree.Hash = "2ac9a6746aca543af8dff39894cfe8173afba21eb01c6fae33d52947222855ef";
            tree.LeftNode.Hash = "000";

            // Act
            var result = _merkleTreeValidator.Validate(tree);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Wrong hash for transaction with id: 1", result.Errors.First());
        }

        [Theory]
        [MemberData(nameof(TransactionDataSet.TransactionData), MemberType = typeof(TransactionDataSet))]
        public void Validate_WrongTree_ErrorValidationResult(HashSet<Transaction> transactions)
        {
            // Arrange
            var tree = new MerkleTreeProvider().GetMerkleTree(transactions);
            tree.Hash = "00000000000000";

            // Act
            var result = _merkleTreeValidator.Validate(tree);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
            Assert.NotEmpty(result.Errors);
        }

        [Theory]
        [MemberData(nameof(TransactionDataSet.TransactionData), MemberType = typeof(TransactionDataSet))]
        public void Validate_CorrectTree_SuccessValidationResult(HashSet<Transaction> transactions)
        {
            // Arrange
            var tree = new MerkleTreeProvider().GetMerkleTree(transactions);

            // Act
            var result = _merkleTreeValidator.Validate(tree);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Errors);
            Assert.Empty(result.Errors);
        }
    }
}