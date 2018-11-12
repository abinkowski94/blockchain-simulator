using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public interface IBlockchainService
    {
        void CreateGenesisBlockIfNotExist();
        
        BaseResponse<BlockBase> GetBlockchainTreeLinked();

        DataAccess.Model.BlockchainTree GetBlockchainTree();

        DataAccess.Model.BlockchainTree GetLongestBlockchain();

        DataAccess.Model.Block.BlockBase GetLastBlock();

        DataAccess.Model.Block.BlockBase GetBlock(string uniqueId);

        DataAccess.Model.BlockchainTreeMetadata GetBlockchainMetadata();

        DataAccess.Model.BlockchainTree GetBlockchainFromBranch(string uniqueId);

        void Clear();

        void AddBlock(DataAccess.Model.Block.BlockBase block);

        bool BlockExists(string blockUniqueId);
    }
}