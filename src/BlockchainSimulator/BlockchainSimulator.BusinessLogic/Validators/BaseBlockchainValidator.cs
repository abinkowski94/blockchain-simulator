using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.Transaction;
using BlockchainSimulator.BusinessLogic.Model.ValidationResults;

namespace BlockchainSimulator.BusinessLogic.Validators
{
    public abstract class BaseBlockchainValidator : IBlockchainValidator
    {
        private readonly IMerkleTreeValidator _merkleTreeValidator;

        public BaseBlockchainValidator(IMerkleTreeValidator merkleTreeValidator)
        {
            _merkleTreeValidator = merkleTreeValidator;
        }
        
        public ValidationResult ValidateTree(Node tree)
        {
            return _merkleTreeValidator.Validate(tree);
        }

        public ValidationResult Validate(Block blockchain)
        {
            var result = SpecificValidation(blockchain);
            return result;
        }

        public abstract ValidationResult SpecificValidation(Block blockchain);
    }
}