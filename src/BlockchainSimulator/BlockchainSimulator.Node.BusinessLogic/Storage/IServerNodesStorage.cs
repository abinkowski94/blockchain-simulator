using System.Collections.Concurrent;
using BlockchainSimulator.Node.BusinessLogic.Model.Consensus;

namespace BlockchainSimulator.Node.BusinessLogic.Storage
{
    public interface IServerNodesStorage
    {
        ConcurrentDictionary<string, ServerNode> ServerNodes { get; }
    }
}