using System.Linq;
using BlockchainSimulator.BusinessLogic.Model.Transaction;
using BlockchainSimulator.BusinessLogic.Model.ValidationResults;
using BlockchainSimulator.BusinessLogic.Services;

namespace BlockchainSimulator.BusinessLogic.Validators
{
    public class MerkleTreeValidator : IMerkleTreeValidator
    {
        private readonly IEncryptionService _encryptionService;

        public MerkleTreeValidator(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        public ValidationResult Validate(MerkleNode tree)
        {
            if (tree.GetType() == typeof(Node))
            {
                var node = (Node) tree;
                var combinedHashes = $"{node.LeftNode.Hash}{node.RightNode.Hash}";
                var hash = _encryptionService.GetSha256Hash(combinedHashes);

                if (hash != node.Hash)
                {
                    return new ValidationResult(false,
                        new[] {$"Wrong hash for nodes hashes h1:{node.LeftNode.Hash} and h2: {node.RightNode.Hash}"});
                }

                var leftNodeValidationResult = Validate(node.LeftNode);
                var rightNodeValidationResult = Validate(node.RightNode);
                
                return new ValidationResult(leftNodeValidationResult.IsSuccess && rightNodeValidationResult.IsSuccess,
                    leftNodeValidationResult.Errors.Concat(rightNodeValidationResult.Errors).ToArray());
            }
            if (tree.GetType() == typeof(Leaf))
            {
                var leaf = (Leaf) tree;
                var hash = _encryptionService.GetSha256Hash(leaf.Transaction.TransactionJson);

                if (leaf.Hash != hash)
                {
                    return new ValidationResult(false,
                        new[] {$"Wrong hash for transaction with id: {leaf.TransactionId}"});
                }

                return new ValidationResult(true, new string[0]);
            }

            return new ValidationResult(false, new[] {"Wrong type of merkle tree node!"});
        }
    }
}