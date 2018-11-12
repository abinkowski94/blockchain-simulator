using System.Collections.Concurrent;
using BlockchainSimulator.Node.BusinessLogic.Model.Staking;
using BlockchainSimulator.Node.DataAccess.Model.Messages;

namespace BlockchainSimulator.Node.BusinessLogic.Storage
{
    public class StakingStorage : IStakingStorage
    {
        public ConcurrentDictionary<int, Epoch> Epochs { get; }
        public ConcurrentBag<TransactionMessage> NodesVotes { get; }

        public StakingStorage()
        {
            Epochs = new ConcurrentDictionary<int, Epoch>();
            NodesVotes = new ConcurrentBag<TransactionMessage>();
        }
    }
}