using BlockchainSimulator.BusinessLogic.Model.Block;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public interface IConsensusService
    {
        void ReachConsensus(BlockBase blockchain);
    }
}