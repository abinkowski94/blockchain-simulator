using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Model.Block;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public interface IBlockchainService
    {
        BlockBase GetBlockchain();

        void SaveBlockchain(BlockBase blockBase, List<BlockBase> blocks = null);
    }
}