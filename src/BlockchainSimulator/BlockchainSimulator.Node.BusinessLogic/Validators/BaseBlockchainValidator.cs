using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.ValidationResults;
using BlockchainSimulator.Node.BusinessLogic.Services;

namespace BlockchainSimulator.Node.BusinessLogic.Validators
{
    public abstract class BaseBlockchainValidator : IBlockchainValidator
    {
        private readonly IMerkleTreeValidator _merkleTreeValidator;

        protected BaseBlockchainValidator(IMerkleTreeValidator merkleTreeValidator)
        {
            _merkleTreeValidator = merkleTreeValidator;
        }

        public ValidationResult Validate(BlockBase blockchain)
        {
            if (blockchain == null)
            {
                return new ValidationResult(false, "The block can not be null!");
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

                blockchain = ((Block)blockchain).Parent;
            }

            validationResult = _merkleTreeValidator.Validate(blockchain.Body.MerkleTree);
            return !validationResult.IsSuccess ? validationResult : SpecificValidation(blockchain);
        }

        protected abstract ValidationResult SpecificValidation(BlockBase blockchain);

        private static ValidationResult ValidateParentHash(Block blockchain)
        {
            var parentHash = EncryptionService.GetSha256Hash(blockchain.Parent.BlockJson);
            return parentHash != blockchain.Header.ParentHash
                ? new ValidationResult(false, $"The parent hash is invalid for block with id: {blockchain.Id}")
                : new ValidationResult(true);
        }
    }
}