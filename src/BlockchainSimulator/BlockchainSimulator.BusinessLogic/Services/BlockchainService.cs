using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.Transaction;
using BlockchainSimulator.BusinessLogic.Model.ValidationResults;
using BlockchainSimulator.BusinessLogic.Providers;
using BlockchainSimulator.BusinessLogic.Validators;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public class BlockchainService : BaseService, IBlockchainService
    {
        private readonly IBlockchainValidator _blockchainValidator;
        private readonly IBlockProvider _blockProvider;

        public BlockchainService(IBlockchainValidator blockchainValidator, IBlockProvider blockProvider)
        {
            _blockchainValidator = blockchainValidator;
            _blockProvider = blockProvider;
        }

        public ValidationResult Validate(BlockBase blockchain)
        {
            return _blockchainValidator.Validate(blockchain);
        }

        public BlockBase CreateBlock(HashSet<Transaction> transactions, BlockBase block = null)
        {
            return _blockProvider.CreateBlock(transactions, block);
        }
    }
}