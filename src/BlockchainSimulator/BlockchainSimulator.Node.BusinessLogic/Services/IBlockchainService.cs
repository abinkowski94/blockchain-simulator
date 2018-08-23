using System.Collections.Generic;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public interface IBlockchainService
    {
        BaseResponse<BlockBase> GetBlockchain();

        void SaveBlockchain(BlockBase blockBase, List<BlockBase> blocks = null);
    }
}