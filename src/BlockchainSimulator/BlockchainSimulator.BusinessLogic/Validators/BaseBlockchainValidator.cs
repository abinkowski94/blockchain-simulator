using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.ValidationResults;
using BlockchainSimulator.BusinessLogic.Services;

namespace BlockchainSimulator.BusinessLogic.Validators
{
    public abstract class BaseBlockchainValidator : IBlockchainValidator
    {
        private readonly IMerkleTreeValidator _merkleTreeValidator;
        protected readonly IEncryptionService _encryptionService;

        public BaseBlockchainValidator(IMerkleTreeValidator merkleTreeValidator, IEncryptionService encryptionService)
        {
            _merkleTreeValidator = merkleTreeValidator;
            _encryptionService = encryptionService;
        }

        protected abstract ValidationResult SpecificValidation(Block blockchain);

        public ValidationResult Validate(Block blockchain)
        {
            while (!blockchain.IsGenesis)
            {
                var validationResult = ValidateParentHash(blockchain);
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }

                validationResult = ValidateTree(blockchain);
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }

                validationResult = SpecificValidation(blockchain);
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }

                blockchain = blockchain.Parent;
            }

            return ValidateTree(blockchain);
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

        private ValidationResult ValidateTree(BlockBase blockchain)
        {
            var validationResult = _merkleTreeValidator.Validate(blockchain.Body.MerkleTree);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            return _merkleTreeValidator.Validate(blockchain.Body.MerkleTree);
        }
    }
}