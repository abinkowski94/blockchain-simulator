using System.Collections.Generic;
using System.Linq;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Model.ValidationResults;
using BlockchainSimulator.Node.BusinessLogic.Services;

namespace BlockchainSimulator.Node.BusinessLogic.Validators
{
    public abstract class BaseBlockchainValidator : IBlockchainValidator
    {
        private readonly IMerkleTreeValidator _merkleTreeValidator;

        protected BaseBlockchainValidator(IMerkleTreeValidator merkleTreeValidator)
        {
            _merkleTreeValidator = merkleTreeValidator;
        }

        protected abstract ValidationResult SpecificValidation(BlockBase blockchain);

        public ValidationResult Validate(BlockBase blockchain)
        {
            if (blockchain == null)
            {
                return new ValidationResult(false, "The block can not be null!");
            }

            var validationResult = new ValidationResult(true);
            while (blockchain != null && !blockchain.IsGenesis)
            {
                SetupTransactions(blockchain.Body.MerkleTree, blockchain.Body.Transactions);

                if (blockchain is Block block && block.Parent != null)
                {
                    validationResult = ValidateParentHash(block);
                    if (!validationResult.IsSuccess)
                    {
                        return validationResult;
                    }
                }

                validationResult = _merkleTreeValidator.Validate(blockchain.Body.MerkleTree);
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }

                validationResult = SpecificValidation(blockchain as Block);
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }

                blockchain = ((Block) blockchain).Parent;
            }

            if (blockchain == null)
            {
                return validationResult;
            }

            SetupTransactions(blockchain.Body.MerkleTree, blockchain.Body.Transactions);
            validationResult = _merkleTreeValidator.Validate(blockchain.Body.MerkleTree);
            return !validationResult.IsSuccess ? validationResult : SpecificValidation(blockchain);
        }

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