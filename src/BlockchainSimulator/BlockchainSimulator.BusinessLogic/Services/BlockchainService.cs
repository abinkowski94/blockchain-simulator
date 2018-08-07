using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.ValidationResults;
using BlockchainSimulator.BusinessLogic.Validators;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public class BlockchainService : BaseService, IBlockchainService
    {
        private readonly IBlockchainValidator _blockchainValidator;

        public BlockchainService(IBlockchainValidator blockchainValidator)
        {
            _blockchainValidator = blockchainValidator;
        }

        public ValidationResult Validate(Block blockchain)
        {
            return _blockchainValidator.Validate(blockchain);
        }
    }
}