using System;
using System.Collections.Generic;
using System.Linq;
using BlockchainSimulator.BusinessLogic.Model.Transaction;
using BlockchainSimulator.BusinessLogic.Providers;
using BlockchainSimulator.BusinessLogic.Providers.Specific;
using BlockchainSimulator.BusinessLogic.Services;
using BlockchainSimulator.BusinessLogic.Tests.Data;
using BlockchainSimulator.BusinessLogic.Validators;
using BlockchainSimulator.BusinessLogic.Validators.Specific;
using Xunit;

namespace BlockchainSimulator.BusinessLogic.Tests.Validators.Specific
{
    public class ProofOfWorkValidatorTests
    {
        private readonly ProofOfWorkValidator _proofOfWorkValidator;
        private readonly ProofOfWorkBlockProvider _proofOfWorkBlockProvider;

        public ProofOfWorkValidatorTests()
        {
            _proofOfWorkValidator = new ProofOfWorkValidator(new MerkleTreeValidator());
            _proofOfWorkBlockProvider = new ProofOfWorkBlockProvider(new MerkleTreeProvider());
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

        [Fact]
        public void Validate_Blockchain_SuccessValidationResult()
        {
            // Arrange
            var genesisTransactions = (HashSet<Transaction>) TransactionDataSet.TransactionData.First().First();
            var blockTransactions = (HashSet<Transaction>) TransactionDataSet.TransactionData.Last().First();

            var genesisBlock = _proofOfWorkBlockProvider.CreateBlock(genesisTransactions);
            var blockchain = _proofOfWorkBlockProvider.CreateBlock(blockTransactions, genesisBlock);

            // Act
            var result = _proofOfWorkValidator.Validate(blockchain);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_Blockchain_ErrorValidationResult()
        {
            // Arrange
            var genesisTransactions = (HashSet<Transaction>) TransactionDataSet.TransactionData.First().First();
            var blockTransactions = (HashSet<Transaction>) TransactionDataSet.TransactionData.Last().First();

            var genesisBlock = _proofOfWorkBlockProvider.CreateBlock(genesisTransactions);
            var blockchain = _proofOfWorkBlockProvider.CreateBlock(blockTransactions, genesisBlock);
            blockchain.Header.Nonce = "0";
            blockchain.Header.TimeStamp = new DateTime(1, 1, 1);

            // Act
            var result = _proofOfWorkValidator.Validate(blockchain);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Single(result.Errors);
        }

        [Theory]
        [MemberData(nameof(TransactionDataSet.TransactionData), MemberType = typeof(TransactionDataSet))]
        public void Validate_GenesisBlocks_SuccessValidationResults(HashSet<Transaction> transactions)
        {
            // Arrange
            var block = _proofOfWorkBlockProvider.CreateBlock(transactions);

            // Act
            var result = _proofOfWorkValidator.Validate(block);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);
        }
    }
}