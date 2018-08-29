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
                return new ValidationResult(true, new string[0]);
            }

            if (tree.GetType() == typeof(Model.Transaction.Node))
            {
                var node = (Model.Transaction.Node)tree;
                var combinedHashes = $"{node.LeftNode.Hash}{node.RightNode?.Hash}";
                var hash = EncryptionService.GetSha256Hash(combinedHashes);

                if (hash != node.Hash)
                {
                    return new ValidationResult(false,
                        new[]
                        {
                            $"Wrong hash for nodes with hashes h1:{node.LeftNode.Hash} and h2: {node.RightNode?.Hash}"
                        });
                }

                var leftNodeValidationResult = Validate(node.LeftNode);
                var rightNodeValidationResult = Validate(node.RightNode);

                return new ValidationResult(leftNodeValidationResult.IsSuccess && rightNodeValidationResult.IsSuccess,
                    leftNodeValidationResult.Errors.Concat(rightNodeValidationResult.Errors).ToArray());
            }

            if (tree.GetType() == typeof(Leaf))
            {
                var leaf = (Leaf)tree;
                var hash = EncryptionService.GetSha256Hash(leaf.Transaction.TransactionJson);

                if (leaf.Hash != hash)
                {
                    return new ValidationResult(false,
                        new[] { $"Wrong hash for transaction with id: {leaf.TransactionId}" });
                }

                return new ValidationResult(true, new string[0]);
            }

            return new ValidationResult(false, new[] { "Wrong type of merkle tree node!" });
        }
    }
}