using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.Responses;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public interface IBlockchainService
    {
        BaseResponse<BlockBase> GetBlockchain();

        void SaveBlockchain(BlockBase blockBase, List<BlockBase> blocks = null);
    }
}