using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.ValidationResults;
using BlockchainSimulator.Node.BusinessLogic.Services;

namespace BlockchainSimulator.Node.BusinessLogic.Validators.Specific
{
    public class ProofOfWorkValidator : BaseBlockchainValidator
    {
        public ProofOfWorkValidator(IMerkleTreeValidator merkleTreeValidator) : base(merkleTreeValidator)
        {
        }

        protected override ValidationResult SpecificValidation(BlockBase blockchain)
        {
            var hash = EncryptionService.GetSha256Hash(blockchain.BlockJson);
            if (hash.StartsWith(blockchain.Header.Target))
            {
                return new ValidationResult(true);
            }

            return new ValidationResult(false,
                $"The hash h: {hash} of block id: {blockchain.Id} does not match the target t: {blockchain.Header.Target}");
        }
    }
}