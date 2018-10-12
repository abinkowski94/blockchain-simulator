using System;
using System.Collections.Concurrent;
using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Node.BusinessLogic.Model.Consensus;

namespace BlockchainSimulator.Node.BusinessLogic.Storage
{
    public class ServerNodesStorage : IServerNodesStorage, IDisposable
    {
        public ConcurrentDictionary<string, ServerNode> ServerNodes { get; }

        public ServerNodesStorage()
        {
            ServerNodes = new ConcurrentDictionary<string, ServerNode>();
        }

        public void Dispose()
        {
            ServerNodes.Values.ForEach(n =>
            {
                n.HubConnection?.StopAsync();
                n.HubConnection?.DisposeAsync();
            });
        }
    }
}