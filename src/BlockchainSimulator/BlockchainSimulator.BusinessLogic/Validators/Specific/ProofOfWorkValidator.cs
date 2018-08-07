using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.ValidationResults;

namespace BlockchainSimulator.BusinessLogic.Validators.Specific
{
    public class ProofOfWorkValidator : BaseBlockchainValidator
    {
        public ProofOfWorkValidator(IMerkleTreeValidator merkleTreeValidator) : base(merkleTreeValidator)
        {
        }

        public override ValidationResult SpecificValidation(Block blockchain)
        {
            // TODO: write the specific validation for proof of work
            return new ValidationResult(true, new string[] { });
        }
    }
}