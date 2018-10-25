using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.MappingProfiles;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Repositories;

namespace BlockchainSimulator.Node.BusinessLogic.Services.Specific
{
    public class ProofOfWorkBlockchainService : BaseService, IBlockchainService
    {
        private readonly IBlockchainRepository _blockchainRepository;

        public ProofOfWorkBlockchainService(IConfigurationService configurationService, IBlockchainRepository blockchainRepository)
            : base(configurationService)
        {
            _blockchainRepository = blockchainRepository;
        }

        public BaseResponse<BlockBase> GetBlockchainTreeLinked()
        {
            var blockchain = _blockchainRepository.GetBlockchainTree();
            if (blockchain?.Blocks == null)
            {
                return new ErrorResponse<BlockBase>("The blockchain tree does not contain blocks!", null);
            }

            var result = LocalMapper.ManualMap(blockchain);
            return new SuccessResponse<BlockBase>("The blockchain from local storage.", result);
        }

        public BlockchainTree GetBlockchainTree()
        {
            return _blockchainRepository.GetBlockchainTree();
        }

        public BlockchainTree GetLongestBlockchain()
        {
            return _blockchainRepository.GetLongestBlockchain();
        }

        public DataAccess.Model.Block.BlockBase GetLastBlock()
        {
            return _blockchainRepository.GetLastBlock();
        }

        public DataAccess.Model.Block.BlockBase GetBlock(string uniqueId)
        {
            return _blockchainRepository.GetBlock(uniqueId);
        }

        public BlockchainTreeMetadata GetBlockchainMetadata()
        {
            return _blockchainRepository.GetBlockchainMetadata();
        }

        public BlockchainTree GetBlockchainFromBranch(string uniqueId)
        {
            return _blockchainRepository.GetBlockchainFromBranch(uniqueId);
        }

        public void Clear()
        {
            _blockchainRepository.Clear();
        }

        public void AddBlock(DataAccess.Model.Block.BlockBase block)
        {
            _blockchainRepository.AddBlock(block);
        }

        public bool BlockExists(string blockUniqueId)
        {
            return _blockchainRepository.BlockExists(blockUniqueId);
        }
    }
}