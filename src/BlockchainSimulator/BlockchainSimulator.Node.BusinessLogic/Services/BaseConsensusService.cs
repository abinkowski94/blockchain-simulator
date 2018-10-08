using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Common.Models.Consensus;
using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Node.BusinessLogic.Hubs;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServerNode = BlockchainSimulator.Node.BusinessLogic.Model.Consensus.ServerNode;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public abstract class BaseConsensusService : BaseService, IConsensusService, IDisposable
    {
        protected readonly ConcurrentDictionary<string, ServerNode> ServerNodes;
        protected readonly IStatisticService StatisticService;
        protected readonly IBackgroundQueue BackgroundQueue;

        protected BaseConsensusService(IStatisticService statisticService, IBackgroundQueue backgroundQueue)
        {
            ServerNodes = new ConcurrentDictionary<string, ServerNode>();
            StatisticService = statisticService;
            BackgroundQueue = backgroundQueue;
        }

        public abstract void AcceptExternalBlock(EncodedBlock encodedBlock);

        public abstract BaseResponse<bool> AcceptBlock(BlockBase blockBase);

        public abstract BaseResponse<bool> SynchronizeWithOtherNodes();

        public BaseResponse<ServerNode> ConnectNode(ServerNode serverNode)
        {
            var validationErrors = ValidateNode(serverNode);
            if (validationErrors.Any())
            {
                return new ErrorResponse<ServerNode>("Invalid input node", serverNode, validationErrors.ToArray());
            }

            serverNode.IsConnected = null;
            CreateNodeConnection(serverNode);

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

        public void Dispose()
        {
            ServerNodes.Values.ForEach(async n =>
            {
                await n.HubConnection.StopAsync();
                await n.HubConnection.DisposeAsync();
            });
        }

        private void CreateNodeConnection(ServerNode serverNode)
        {
            BackgroundQueue.Enqueue(async token =>
            {
                var url = $"{serverNode.HttpAddress}/consensusHub";
                serverNode.HubConnection = new HubConnectionBuilder().WithUrl(url).Build();

                // Reconnect when connection is closing
                serverNode.HubConnection.Closed += async error =>
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000, token);
                    await serverNode.HubConnection.StartAsync(token);
                };

                // Register action: acceptance of external block
                const string methodName = nameof(IConsensusClient.ReceiveBlock);
                serverNode.HubConnection.On<EncodedBlock>(methodName, AcceptExternalBlock);

                // Start the connection
                await serverNode.HubConnection.StartAsync(token);

                // Sets the connection status
                serverNode.IsConnected = true;
            });
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
                validationErrors.Add("The node's HTTP address cannot be null!");
            }

            if (serverNode.Id != null && ServerNodes.ContainsKey(serverNode.Id))
            {
                validationErrors.Add($"The node's id already exists id: {serverNode.Id}!");
            }

            return validationErrors;
        }
    }
}