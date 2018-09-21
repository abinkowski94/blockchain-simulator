using BlockchainSimulator.Node.BusinessLogic.Configurations;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Providers;
using BlockchainSimulator.Node.BusinessLogic.Providers.Specific;
using BlockchainSimulator.Node.BusinessLogic.Tests.Data;
using BlockchainSimulator.Node.BusinessLogic.Validators;
using BlockchainSimulator.Node.BusinessLogic.Validators.Specific;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BlockchainSimulator.Node.BusinessLogic.Tests.Validators.Specific
{
    public class ProofOfWorkValidatorTests
    {
        private readonly ProofOfWorkBlockProvider _proofOfWorkBlockProvider;
        private readonly ProofOfWorkValidator _proofOfWorkValidator;

        public ProofOfWorkValidatorTests()
        {
            var configurationMock = new Mock<IBlockchainConfiguration>();
            configurationMock.Setup(p => p.Target).Returns("0000");
            configurationMock.Setup(p => p.Version).Returns("PoW-v1");
            configurationMock.Setup(p => p.BlockSize).Returns(10);
            var configuration = configurationMock.Object;

            _proofOfWorkValidator = new ProofOfWorkValidator(new MerkleTreeValidator());
            _proofOfWorkBlockProvider = new ProofOfWorkBlockProvider(new MerkleTreeProvider(), configuration,
                new Mock<IConfiguration>().Object);
        }

        [Fact]
        public void Validate_Blockchain_ErrorValidationResult()
        {
            // Arrange
            var genesisTransactions = (HashSet<Transaction>)TransactionDataSet.TransactionData.First().First();
            var blockTransactions = (HashSet<Transaction>)TransactionDataSet.TransactionData.Last().First();

            var genesisBlock = _proofOfWorkBlockProvider.CreateBlock(genesisTransactions, new DateTime(1, 1, 1));
            var blockchain =
                _proofOfWorkBlockProvider.CreateBlock(blockTransactions, new DateTime(1, 1, 1), genesisBlock);
            blockchain.Header.Nonce = "0";
            blockchain.Header.TimeStamp = new DateTime(1, 1, 1);

            // Act
            var result = _proofOfWorkValidator.Validate(blockchain);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void Validate_Blockchain_SuccessValidationResult()
        {
            // Arrange
            var genesisTransactions = (HashSet<Transaction>)TransactionDataSet.TransactionData.First().First();
            var blockTransactions = (HashSet<Transaction>)TransactionDataSet.TransactionData.Last().First();

            var genesisBlock = _proofOfWorkBlockProvider.CreateBlock(genesisTransactions, new DateTime(1, 1, 1));
            var blockchain =
                _proofOfWorkBlockProvider.CreateBlock(blockTransactions, new DateTime(1, 1, 1), genesisBlock);

            // Act
            var result = _proofOfWorkValidator.Validate(blockchain);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);
        }

        [Theory]
        [MemberData(nameof(TransactionDataSet.TransactionData), MemberType = typeof(TransactionDataSet))]
        public void Validate_GenesisBlocks_SuccessValidationResults(HashSet<Transaction> transactions)
        {
            // Arrange
            var block = _proofOfWorkBlockProvider.CreateBlock(transactions, new DateTime(1, 1, 1));

            // Act
            var result = _proofOfWorkValidator.Validate(block);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_Null_ErrorValidationResult()
        {
            // Arrange

            // Act
            var result = _proofOfWorkValidator.Validate(null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Single(result.Errors);
            Assert.Equal("The block can not be null!", result.Errors.First());
        }
    }
}