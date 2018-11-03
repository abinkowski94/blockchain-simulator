using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.ValidationResults;

namespace BlockchainSimulator.Node.BusinessLogic.Validators.Specific
{
    public class ProofOfStakeValidator : BaseBlockchainValidator
    {
        public ProofOfStakeValidator(IMerkleTreeValidator merkleTreeValidator) : base(merkleTreeValidator)
        {
        }

        protected override ValidationResult SpecificValidation(BlockBase blockchain)
        {
            return new ValidationResult(true);
        }
    }
}