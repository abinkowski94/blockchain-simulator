using BlockchainSimulator.Common.Models.Consensus;
using BlockchainSimulator.Common.Services;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServerNode = BlockchainSimulator.Node.BusinessLogic.Model.Consensus.ServerNode;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public abstract class BaseConsensusService : BaseService, IConsensusService
    {
        protected readonly ConcurrentDictionary<string, ServerNode> ServerNodes;

        protected readonly IBackgroundTaskQueue Queue;
        protected readonly IHttpService HttpService;

        protected BaseConsensusService(IBackgroundTaskQueue queue, IHttpService httpService)
        {
            ServerNodes = new ConcurrentDictionary<string, ServerNode>();

            HttpService = httpService;
            Queue = queue;
        }

        public abstract void AcceptExternalBlock(EncodedBlock encodedBlock);

        public abstract BaseResponse<bool> AcceptBlock(BlockBase blockBase);

        public BaseResponse<ServerNode> ConnectNode(ServerNode serverNode)
        {
            var validationErrors = ValidateNode(serverNode);
            if (validationErrors.Any())
            {
                return new ErrorResponse<ServerNode>("Invalid input node", serverNode, validationErrors.ToArray());
            }

            serverNode.IsConnected = null;
            CheckNodeConnection(serverNode);

            if (!ServerNodes.TryAdd(serverNode.Id, serverNode))
            {
                return new ErrorResponse<ServerNode>($"Could not add a node with id: {serverNode.Id}", serverNode);
            }

            return new SuccessResponse<ServerNode>("The node has been added successfully!", serverNode);
        }

        public BaseResponse<List<ServerNode>> DisconnectFromNetwork()
        {
            var result = ServerNodes.Select(kv => kv.Value).OrderBy(n => n.Delay).ToList();
            result.ForEach(n => n.IsConnected = false);
            ServerNodes.Clear();

            return new SuccessResponse<List<ServerNode>>("All nodes has been disconnected!", result);
        }

        public BaseResponse<ServerNode> DisconnectNode(string nodeId)
        {
            ServerNodes.TryRemove(nodeId, out var result);
            if (result == null)
            {
                return new ErrorResponse<ServerNode>($"Could not disconnect node with id: {nodeId}", null);
            }

            result.IsConnected = false;
            return new SuccessResponse<ServerNode>($"The node with id: {nodeId} has been disconnected!", result);
        }

        public BaseResponse<List<ServerNode>> GetNodes()
        {
            return new SuccessResponse<List<ServerNode>>($"The server list for current time: {DateTime.UtcNow}",
                ServerNodes.Select(kv => kv.Value).OrderBy(n => n.Delay).ToList());
        }

        private void CheckNodeConnection(ServerNode serverNode)
        {
            Queue.EnqueueTask(token => Task.Run(() =>
            {
                var timeout = TimeSpan.FromSeconds(30);
                var response = HttpService.Get($"{serverNode.HttpAddress}/api/info", timeout, token);
                serverNode.IsConnected = response.IsSuccessStatusCode;
            }, token));
        }

        private List<string> ValidateNode(ServerNode serverNode)
        {
            var validationErrors = new List<string>();
            if (serverNode == null)
            {
                validationErrors.Add("The node can not be null!");
                return validationErrors;
            }

            if (serverNode.Id == null)
            {
                validationErrors.Add("The node's id can not be null!");
            }

            if (serverNode.HttpAddress == null)
            {
                validationErrors.Add("The node's http address cannot be null!");
            }

            if (serverNode.Id != null && ServerNodes.ContainsKey(serverNode.Id))
            {
                validationErrors.Add($"The node's id already exists id: {serverNode.Id}!");
            }

            return validationErrors;
        }
    }
}