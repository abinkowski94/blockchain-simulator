using System.Collections.Concurrent;
using BlockchainSimulator.Node.BusinessLogic.Model.Staking;

namespace BlockchainSimulator.Node.BusinessLogic.Storage
{
    public interface IStakingStorage
    {
        ConcurrentDictionary<int, Epoch> Epochs { get; }
        ConcurrentBag<string> NodesVotes { get; }
    }
}