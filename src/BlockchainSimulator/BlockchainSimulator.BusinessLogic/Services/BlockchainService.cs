using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.Maps;
using BlockchainSimulator.BusinessLogic.Model.Transaction;
using BlockchainSimulator.BusinessLogic.Model.ValidationResults;
using BlockchainSimulator.BusinessLogic.Providers;
using BlockchainSimulator.BusinessLogic.Validators;
using BlockchainSimulator.DataAccess.Repositories;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public class BlockchainService : BaseService, IBlockchainService
    {
        private readonly IBlockchainRepository _blockchainRepository;
        private readonly IBlockchainValidator _blockchainValidator;
        private readonly IBlockProvider _blockProvider;

        public BlockchainService(IBlockchainValidator blockchainValidator, IBlockProvider blockProvider,
            IBlockchainRepository blockchainRepository)
        {
            _blockchainValidator = blockchainValidator;
            _blockProvider = blockProvider;
            _blockchainRepository = blockchainRepository;
        }

        public ValidationResult Validate(BlockBase blockchain)
        {
            return _blockchainValidator.Validate(blockchain);
        }

        public BlockBase CreateBlock(HashSet<Transaction> transactions, BlockBase block = null)
        {
            return _blockProvider.CreateBlock(transactions, block);
        }

        public BlockBase GetBlockchain()
        {
            var blockchain = _blockchainRepository.GetBlockchain();
            return LocalMapper.ManualMap(blockchain);
        }
    }
}