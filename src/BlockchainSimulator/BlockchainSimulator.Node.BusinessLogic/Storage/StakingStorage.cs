using System.Collections.Concurrent;
using BlockchainSimulator.Node.BusinessLogic.Model.Staking;

namespace BlockchainSimulator.Node.BusinessLogic.Storage
{
    public class StakingStorage : IStakingStorage
    {
        public ConcurrentDictionary<int, Epoch> Epochs { get; }
        public ConcurrentBag<string> NodesVotes { get; }

        public StakingStorage()
        {
            Epochs = new ConcurrentDictionary<int, Epoch>();
            NodesVotes = new ConcurrentBag<string>();
        }

        public void Clear()
        {
            Epochs.Clear();
            NodesVotes.Clear();
        }
    }
}