using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.ValidationResults;
using BlockchainSimulator.BusinessLogic.Services;

namespace BlockchainSimulator.BusinessLogic.Validators.Specific
{
    public class ProofOfWorkValidator : BaseBlockchainValidator
    {
        public ProofOfWorkValidator(IMerkleTreeValidator merkleTreeValidator, IEncryptionService encryptionService) :
            base(merkleTreeValidator, encryptionService)
        {
        }

        protected override ValidationResult SpecificValidation(Block blockchain)
        {
            var hash = _encryptionService.GetSha256Hash(blockchain.BlockJson);
            if (hash.StartsWith(blockchain.Header.Target))
            {
                return new ValidationResult(true, new string[] { });
            }

            return new ValidationResult(false, new[]
            {
                $"The hash h: {hash} of block id: {blockchain.Id} does not match the target t: {blockchain.Header.Target}"
            });
        }
    }
}