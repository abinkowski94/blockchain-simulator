using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.MappingProfiles;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.DataAccess.Repositories;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class BlockchainService : BaseService, IBlockchainService
    {
        private readonly IBlockchainRepository _blockchainRepository;

        public BlockchainService(IConfigurationService configurationService, IBlockchainRepository blockchainRepository)
            : base(configurationService)
        {
            _blockchainRepository = blockchainRepository;
        }

        public BaseResponse<BlockBase> GetBlockchainTree()
        {
            var blockchain = _blockchainRepository.GetBlockchainTree();
            if (blockchain?.Blocks == null)
            {
                return new ErrorResponse<BlockBase>("The blockchain tree does not contain blocks!", null);
            }

            var result = LocalMapper.ManualMap(blockchain);
            return new SuccessResponse<BlockBase>("The blockchain from local storage.", result);
        }
    }
}