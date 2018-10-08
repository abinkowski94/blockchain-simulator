using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Common.Models.Consensus;
using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Node.BusinessLogic.Hubs;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Validators;
using BlockchainSimulator.Node.DataAccess.Converters;
using BlockchainSimulator.Node.DataAccess.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAM = BlockchainSimulator.Node.DataAccess.Model;

namespace BlockchainSimulator.Node.BusinessLogic.Services.Specific
{
    public class ProofOfWorkConsensusService : BaseConsensusService
    {
        private readonly ConcurrentBag<string> _encodedBlocksIds;
        private readonly string _nodeId;

        private readonly IHubContext<ConsensusHub, IConsensusClient> _consensusHubContext;
        private readonly IBlockchainRepository _blockchainRepository;
        private readonly IBlockchainValidator _blockchainValidator;

        public ProofOfWorkConsensusService(IBackgroundQueue backgroundQueue, IConfiguration configuration,
            IBlockchainRepository blockchainRepository, IBlockchainValidator blockchainValidator,
            IStatisticService statisticService, IHubContext<ConsensusHub, IConsensusClient> consensusHubContext)
            : base(statisticService, backgroundQueue)
        {
            _encodedBlocksIds = new ConcurrentBag<string>();
            _nodeId = configuration["Node:Id"];

            _consensusHubContext = consensusHubContext;
            _blockchainRepository = blockchainRepository;
            _blockchainValidator = blockchainValidator;
        }

        public override void AcceptExternalBlock(EncodedBlock encodedBlock)
        {
            BackgroundQueue.Enqueue(token => new Task(() =>
            {
                if (encodedBlock?.Base64Block != null && !_encodedBlocksIds.Contains(encodedBlock.Id))
                {
                    _encodedBlocksIds.Add(encodedBlock.Id);

                    var blockchainJson = Encoding.UTF8.GetString(Convert.FromBase64String(encodedBlock.Base64Block));
                    var incomingBlock = BlockchainConverter.DeserializeBlock(blockchainJson);

                    var result = AcceptBlock(incomingBlock, encodedBlock.NodeSenderId);
                    if (result.IsSuccess)
                    {
                        DistributeBlock(encodedBlock);
                    }
                }
            }, token));
        }

        public override BaseResponse<bool> AcceptBlock(BlockBase blockBase)
        {
            if (blockBase == null)
            {
                return new ErrorResponse<bool>("The block can not be null!", false);
            }

            var lastBlock = _blockchainRepository.GetLastBlock();
            if (blockBase is Block block && block.ParentUniqueId != lastBlock.UniqueId)
            {
                return new ErrorResponse<bool>("The blockchain head has changed!", false);
            }

            var validationResult = ValidateTransactionsDuplicates(blockBase);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            var mappedBlock = LocalMapper.Map<DAM.Block.BlockBase>(blockBase);
            _blockchainRepository.AddBlock(mappedBlock);
            DistributeBlock(mappedBlock);

            return new SuccessResponse<bool>("The block has been accepted and appended!", true);
        }

        public override BaseResponse<bool> SynchronizeWithOtherNodes()
        {
            var blocks = new ConcurrentDictionary<string, DAM.Block.BlockBase>();
            ServerNodes.Values.ParallelForEach(node =>
            {
                var json = node.HubConnection.Invoke<string>(nameof(ConsensusHub.GetLastBlockJson));
                if (json != null)
                {
                    blocks.TryAdd(node.Id, BlockchainConverter.DeserializeBlock(json));
                }
            });

            var currentLastBlock = _blockchainRepository.GetLastBlock();
            if (blocks.IsEmpty && currentLastBlock == null)
            {
                return new ErrorResponse<bool>(
                    "The current blockchain is empty and there is no block to synchronize", false);
            }

            if (blocks.IsEmpty)
            {
                return new ErrorResponse<bool>("There is no blocks to synchronize with!", false);
            }

            var longestBlock = blocks.OrderBy(b => b.Value.Depth).ThenBy(b => b.Value.Header.TimeStamp).First();
            if (currentLastBlock == null)
            {
                return AcceptBlock(longestBlock.Value, longestBlock.Key);
            }

            return currentLastBlock.Depth >= longestBlock.Value.Depth
                ? new SuccessResponse<bool>("There is no need for synchronization!", false)
                : AcceptBlock(longestBlock.Value, longestBlock.Key);
        }

        private BaseResponse<bool> AcceptBlock(DAM.Block.BlockBase incomingBlock, string senderNodeId)
        {
            if (incomingBlock == null)
            {
                return new ErrorResponse<bool>("The incoming block cannot be null!", false);
            }

            if (_blockchainRepository.BlockExists(incomingBlock.UniqueId))
            {
                StatisticService.RegisterRejectedBlock();
                return new ErrorResponse<bool>("The block already exists!", false);
            }

            var mappedBlock = LocalMapper.Map<BlockBase>(incomingBlock);
            if (incomingBlock is DAM.Block.Block block && !incomingBlock.IsGenesis)
            {
                var parentBlockValidationResult = ValidateParentBlock(block, senderNodeId);
                if (!parentBlockValidationResult.IsSuccess)
                {
                    StatisticService.RegisterRejectedBlock();
                    return parentBlockValidationResult;
                }

                var parentBlock = _blockchainRepository.GetBlock(block.ParentUniqueId);
                if (parentBlock == null || parentBlock.Depth + 1 != mappedBlock.Depth)
                {
                    StatisticService.RegisterRejectedBlock();
                    return new ErrorResponse<bool>("The depth of incoming block is incorrect!", false);
                }

                var mappedParentBlock = LocalMapper.Map<BlockBase>(parentBlock);
                ((Block) mappedBlock).Parent = mappedParentBlock;
            }

            var duplicatesValidationResult = ValidateTransactionsDuplicates(mappedBlock);
            if (!duplicatesValidationResult.IsSuccess)
            {
                return duplicatesValidationResult;
            }

            var validationResult = _blockchainValidator.Validate(mappedBlock);
            if (!validationResult.IsSuccess)
            {
                StatisticService.RegisterRejectedBlock();
                return new ErrorResponse<bool>("The validation for the block failed!", false, validationResult.Errors);
            }

            _blockchainRepository.AddBlock(incomingBlock);
            return new SuccessResponse<bool>("The block has been accepted and appended!", true);
        }

        private void DistributeBlock(DAM.Block.BlockBase blockBase)
        {
            var blockJson = JsonConvert.SerializeObject(blockBase);
            var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(blockJson));
            var encodedBlock = new EncodedBlock
            {
                Id = Guid.NewGuid().ToString(),
                Base64Block = base64String,
                NodeSenderId = _nodeId,
                NodesAcceptedIds = new List<string>()
            };

            DistributeBlock(encodedBlock);
        }

        private async void DistributeBlock(EncodedBlock encodedBlock)
        {
            encodedBlock.NodeSenderId = _nodeId;
            encodedBlock.NodesAcceptedIds.Add(_nodeId);

            await _consensusHubContext.Clients.All.ReceiveBlock(encodedBlock);
        }

        private BaseResponse<bool> ValidateTransactionsDuplicates(BlockBase blockBase)
        {
            var blockchainBranch = _blockchainRepository.GetBlockchainFromBranch((blockBase as Block)?.ParentUniqueId);
            if (blockchainBranch != null)
            {
                var transactionsIds = blockchainBranch.Blocks.SelectMany(b => b.Body.Transactions).Select(t => t.Id)
                    .ToList();

                if (blockBase.Body.Transactions.Any(t => transactionsIds.Contains(t.Id)))
                {
                    return new ErrorResponse<bool>("One of the transactions already exists in main chain!", false);
                }
            }

            return new SuccessResponse<bool>("The transactions are valid!", true);
        }

        private BaseResponse<bool> ValidateParentBlock(DAM.Block.Block block, string senderNodeId)
        {
            if (_blockchainRepository.BlockExists(block.ParentUniqueId))
            {
                return new SuccessResponse<bool>("The parent block exists!", true);
            }

            var serverNode = ServerNodes.Values.FirstOrDefault(n => n.Id == senderNodeId);
            if (serverNode == null)
            {
                return new ErrorResponse<bool>(
                    $"Could not find the server for parent block with id: {block.ParentUniqueId} don't exist!", false);
            }

            var methodName = nameof(ConsensusHub.GetBlocksFromBranchJson);
            var json = serverNode.HubConnection.Invoke<string>(methodName, block.ParentUniqueId);
            if (json != null)
            {
                var externalBlocks = BlockchainConverter.DeserializeBlocks(json).OrderBy(b => b.Depth);
                var results = externalBlocks.Select(eb => AcceptBlock(eb, senderNodeId));
                if (results.All(r => r.IsSuccess || r.Message == "The block already exists!"))
                {
                    return new SuccessResponse<bool>("The parent block exists!", true);
                }
            }

            return new ErrorResponse<bool>($"The parent block with id: {block.ParentUniqueId} don't exist!", false);
        }
    }
}