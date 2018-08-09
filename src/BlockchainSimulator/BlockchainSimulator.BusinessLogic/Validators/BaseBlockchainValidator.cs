using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.ValidationResults;
using BlockchainSimulator.BusinessLogic.Services;

namespace BlockchainSimulator.BusinessLogic.Validators
{
    public abstract class BaseBlockchainValidator : IBlockchainValidator
    {
        private readonly IMerkleTreeValidator _merkleTreeValidator;
        protected readonly IEncryptionService _encryptionService;

        protected BaseBlockchainValidator(IMerkleTreeValidator merkleTreeValidator,
            IEncryptionService encryptionService)
        {
            _merkleTreeValidator = merkleTreeValidator;
            _encryptionService = encryptionService;
        }

        protected abstract ValidationResult SpecificValidation(BlockBase blockchain);

        public ValidationResult Validate(BlockBase blockchain)
        {
            if (blockchain == null)
            {
                return new ValidationResult(false, new[] {"The block can not be null!"});
            }

            ValidationResult validationResult;
            while (!blockchain.IsGenesis)
            {
                validationResult = ValidateParentHash(blockchain as Block);
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
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

            validationResult = _merkleTreeValidator.Validate(blockchain.Body.MerkleTree);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            return SpecificValidation(blockchain);
        }

        private ValidationResult ValidateParentHash(Block blockchain)
        {
            var parentHash = _encryptionService.GetSha256Hash(blockchain.Parent.BlockJson);
            if (parentHash != blockchain.Header.ParentHash)
            {
                return new ValidationResult(false,
                    new[] {$"The parent hash is invalid for block with id: {blockchain.Id}"});
            }

            return new ValidationResult(true, new string[0]);
        }
    }
}