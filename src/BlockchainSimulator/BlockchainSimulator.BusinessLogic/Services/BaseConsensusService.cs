using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using BlockchainSimulator.BusinessLogic.Model.Consensus;
using BlockchainSimulator.BusinessLogic.Queue;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public abstract class BaseConsensusService : BaseService, IConsensusService
    {
        protected readonly ConcurrentDictionary<string, ServerNode> _serverNodes;
        protected readonly IBackgroundTaskQueue _queue;

        protected BaseConsensusService(IBackgroundTaskQueue queue)
        {
            _serverNodes = new ConcurrentDictionary<string, ServerNode>();
            _queue = queue;
        }

        public abstract bool AcceptBlockchain(string base64Blockchain);

        public abstract void ReachConsensus();

        public List<ServerNode> GetNodes()
        {
            return _serverNodes.Select(kv => kv.Value).ToList();
        }

        public ServerNode ConnectNode(ServerNode serverNode)
        {
            if (serverNode?.Id == null || serverNode.HttpAddress == null || _serverNodes.ContainsKey(serverNode.Id))
            {
                return null;
            }

            _queue.QueueBackgroundWorkItem(async token =>
            {
                using (var httpClientHandler = new HttpClientHandler())
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback =
                        (message, cert, chain, errors) => true;
                    using (var httpClient = new HttpClient(httpClientHandler))
                    {
                        try
                        {
                            var result = await httpClient.GetAsync($"{serverNode.HttpAddress}/api/info", token);
                            serverNode.IsConnected = result.IsSuccessStatusCode;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
            });

            return _serverNodes.TryAdd(serverNode.Id, serverNode) ? serverNode : null;
        }

        public ServerNode DisconnectNode(string nodeId)
        {
            _serverNodes.TryRemove(nodeId, out var result);
            if (result != null)
            {
                result.IsConnected = false;
            }

            return result;
        }

        public List<ServerNode> DisconnectFromNetwork()
        {
            var result = _serverNodes.Select(kv => kv.Value).ToList();
            result.ForEach(n => n.IsConnected = false);
            _serverNodes.Clear();

            return result;
        }
    }
}