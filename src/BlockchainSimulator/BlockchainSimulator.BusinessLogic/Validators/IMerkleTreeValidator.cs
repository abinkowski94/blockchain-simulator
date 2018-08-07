using BlockchainSimulator.BusinessLogic.Model.Transaction;
using BlockchainSimulator.BusinessLogic.Model.ValidationResults;

namespace BlockchainSimulator.BusinessLogic.Validators
{
    public interface IMerkleTreeValidator
    {
        ValidationResult Validate(Node tree);
    }
}