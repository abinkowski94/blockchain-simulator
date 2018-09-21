using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Model.ValidationResults;
using BlockchainSimulator.Node.BusinessLogic.Services;
using System.Collections.Generic;
using System.Linq;

namespace BlockchainSimulator.Node.BusinessLogic.Validators
{
    public abstract class BaseBlockchainValidator : IBlockchainValidator
    {
        private readonly IMerkleTreeValidator _merkleTreeValidator;

        protected BaseBlockchainValidator(IMerkleTreeValidator merkleTreeValidator)
        {
            _merkleTreeValidator = merkleTreeValidator;
        }

        public ValidationResult Validate(BlockBase blockchain)
        {
            if (blockchain == null)
            {
                return new ValidationResult(false, "The block can not be null!");
            }

            var validationResult = new ValidationResult(true);
            while (blockchain != null && blockchain is Block block)
            {
                SetupTransactions(block.Body.MerkleTree, block.Body.Transactions);
                if (block.Parent != null)
                {
                    validationResult = ValidateParentHash(block);
                    if (!validationResult.IsSuccess)
                    {
                        return validationResult;
                    }
                }

                validationResult = _merkleTreeValidator.Validate(block.Body.MerkleTree);
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }

                validationResult = SpecificValidation(block);
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }

                blockchain = block.Parent;
            }

            if (blockchain == null)
            {
                return validationResult;
            }

            SetupTransactions(blockchain.Body.MerkleTree, blockchain.Body.Transactions);
            validationResult = _merkleTreeValidator.Validate(blockchain.Body.MerkleTree);
            return !validationResult.IsSuccess ? validationResult : SpecificValidation(blockchain);
        }

        protected abstract ValidationResult SpecificValidation(BlockBase blockchain);

        private static ValidationResult ValidateParentHash(Block blockchain)
        {
            var parentHash = EncryptionService.GetSha256Hash(blockchain.Parent.BlockJson);
            return parentHash != blockchain.Header.ParentHash
                ? new ValidationResult(false, $"The parent hash is invalid for block with id: {blockchain.Id}")
                : new ValidationResult(true);
        }

        private static void SetupTransactions(MerkleNode merkleNode, HashSet<Transaction> transactions)
        {
            if (merkleNode is Leaf leaf)
            {
                leaf.Transaction = transactions.FirstOrDefault(t => t.Id == leaf.TransactionId);
            }
            else if (merkleNode is Model.Transaction.Node node)
            {
                SetupTransactions(node.LeftNode, transactions);
                SetupTransactions(node.RightNode, transactions);
            }
        }
    }
}