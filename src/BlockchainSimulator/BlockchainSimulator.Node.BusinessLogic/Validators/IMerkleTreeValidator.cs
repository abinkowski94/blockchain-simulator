using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Model.ValidationResults;

namespace BlockchainSimulator.Node.BusinessLogic.Validators
{
    public interface IMerkleTreeValidator
    {
        ValidationResult Validate(MerkleNode tree);
    }
}