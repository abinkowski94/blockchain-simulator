using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Model.ValidationResults;
using BlockchainSimulator.Node.BusinessLogic.Services;
using System.Linq;

namespace BlockchainSimulator.Node.BusinessLogic.Validators
{
    public class MerkleTreeValidator : IMerkleTreeValidator
    {
        public ValidationResult Validate(MerkleNode tree)
        {
            if (tree == null)
            {
                return new ValidationResult(true);
            }

            if (tree.GetType() == typeof(Model.Transaction.Node))
            {
                var node = (Model.Transaction.Node)tree;
                var combinedHashes = $"{node.LeftNode.Hash}{node.RightNode?.Hash}";
                var hash = EncryptionService.GetSha256Hash(combinedHashes);

                if (hash != node.Hash)
                {
                    return new ValidationResult(false,
                        $"Wrong hash for nodes with hashes h1:{node.LeftNode.Hash} and h2: {node.RightNode?.Hash}");
                }

                var leftNodeValidationResult = Validate(node.LeftNode);
                var rightNodeValidationResult = Validate(node.RightNode);

                return new ValidationResult(leftNodeValidationResult.IsSuccess && rightNodeValidationResult.IsSuccess,
                    leftNodeValidationResult.Errors.Concat(rightNodeValidationResult.Errors).ToArray());
            }

            if (tree.GetType() != typeof(Leaf))
            {
                return new ValidationResult(false, "Wrong type of merkle tree node!");
            }

            var leaf = (Leaf)tree;
            var transactionJson = leaf.Transaction.TransactionJson;
            var transactionHash = EncryptionService.GetSha256Hash(transactionJson);

            return leaf.Hash != transactionHash
                ? new ValidationResult(false, $"Wrong hash for transaction with id: {leaf.TransactionId}")
                : new ValidationResult(true);
        }
    }
}