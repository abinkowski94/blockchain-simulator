using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Common.Services;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Validators;
using BlockchainSimulator.Node.DataAccess.Converters;
using BlockchainSimulator.Node.DataAccess.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlockchainSimulator.Common.Models.Consensus;
using BlockchainSimulator.Common.Models.WebClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Block = BlockchainSimulator.Node.DataAccess.Model.Block.Block;

namespace BlockchainSimulator.Node.BusinessLogic.Services.Specific
{
    public class ProofOfWorkConsensusService : BaseConsensusService
    {
        private readonly ConcurrentBag<string> _encodedBlocksIds;
        private readonly string _nodeId;

        private readonly IBlockchainRepository _blockchainRepository;
        private readonly IBlockchainValidator _blockchainValidator;
        private readonly IStatisticService _statisticService;

        public ProofOfWorkConsensusService(IBackgroundTaskQueue queue, IHttpService httpService,
            IBlockchainRepository blockchainRepository, IBlockchainValidator blockchainValidator,
            IStatisticService statisticService, IConfiguration configuration) : base(queue, httpService)
        {
            _encodedBlocksIds = new ConcurrentBag<string>();
            _nodeId = configuration["Node:Id"];

            _blockchainRepository = blockchainRepository;
            _blockchainValidator = blockchainValidator;
            _statisticService = statisticService;
        }

        public override void AcceptExternalBlock(EncodedBlock encodedBlock)
        {
            _queue.QueueBackgroundWorkItem(token => new Task<BaseResponse<bool>>(() =>
            {
                if (encodedBlock?.Base64Block == null)
                {
                    _statisticService.RegisterRejectedBlock();
                    return new ErrorResponse<bool>("The block can not be null!", false);
                }

                if (_encodedBlocksIds.Contains(encodedBlock.Id))
                {
                    return new ErrorResponse<bool>("The encoded block has been already accepted!", false);
                }

                var blockchainJson = Encoding.UTF8.GetString(Convert.FromBase64String(encodedBlock.Base64Block));
                var incomingBlock = BlockchainConverter.DeserializeBlock(blockchainJson);

                var result = AcceptBlock(incomingBlock, encodedBlock.NodeSenderId);
                if (result.IsSuccess)
                {
                    _encodedBlocksIds.Add(encodedBlock.Id);
                    DistributeBlock(encodedBlock);
                }

                return result;
            }, token));
        }

        public override BaseResponse<bool> AcceptBlock(BlockBase blockBase)
        {
            // Warning call this only withing queue
            if (blockBase == null)
            {
                return new ErrorResponse<bool>("The block can not be null!", false);
            }

            var lastBlock = _blockchainRepository.GetLastBlock();
            if (blockBase is Model.Block.Block block && block.ParentUniqueId != lastBlock.UniqueId)
            {
                return new ErrorResponse<bool>("The blockchain head has changed!", false);
            }

            var validationResult = ValidateTransactionsDuplicates(blockBase);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            var mappedBlock = LocalMapper.Map<DataAccess.Model.Block.BlockBase>(blockBase);
            _blockchainRepository.AddBlock(mappedBlock);
            DistributeBlock(mappedBlock);

            return new SuccessResponse<bool>("The block has been accepted and appended!", true);
        }

        public override void SynchronizeWithOtherNodes()
        {
            _queue.QueueBackgroundWorkItem(token => new Task(() =>
            {
                var currentBlocksIds = _blockchainRepository.GetBlocksIds();
                var externalBlocks =
                    new ConcurrentDictionary<string, Tuple<DataAccess.Model.Block.BlockBase, string>>();
                _serverNodes.Values.ParallelForEach(node =>
                {
                    var httpResponse = _httpService.Get($"{node.HttpAddress}/api/blockchain/ids");
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        var externalIds = httpResponse.Content.ReadAs<List<string>>();
                        if (externalIds != null)
                        {
                            var blockIdsToSync = externalIds.Where(eid => !currentBlocksIds.Contains(eid)).ToList();
                            var httpContent = new JsonContent(blockIdsToSync);
                            httpResponse = _httpService.Post($"{node.HttpAddress}/api/blockchain/ids", httpContent);
                            if (httpResponse.IsSuccessStatusCode)
                            {
                                var externalBlocksJson = httpResponse.Content.ReadAsString();
                                var externalBlocksList = BlockchainConverter.DeserializeBlocks(externalBlocksJson);
                                externalBlocksList?.ForEach(b => externalBlocks.TryAdd(b.UniqueId,
                                    new Tuple<DataAccess.Model.Block.BlockBase, string>(b, node.Id)));
                            }
                        }
                    }
                });

                externalBlocks.Values.OrderBy(t => t.Item1.Depth).ForEach(b => AcceptBlock(b.Item1, b.Item2));
            }, token));
        }

        private BaseResponse<bool> AcceptBlock(DataAccess.Model.Block.BlockBase incomingBlock, string senderNodeId)
        {
            if (_blockchainRepository.BlockExists(incomingBlock.UniqueId))
            {
                _statisticService.RegisterRejectedBlock();
                return new ErrorResponse<bool>("The block already exists!", false);
            }

            var mappedBlock = LocalMapper.Map<BlockBase>(incomingBlock);

            var duplicatesValidationResult = ValidateTransactionsDuplicates(mappedBlock);
            if (!duplicatesValidationResult.IsSuccess)
            {
                return duplicatesValidationResult;
            }

            if (incomingBlock is Block block)
            {
                var parentBlockValidationResult = ValidateParentBlock(block, senderNodeId);
                if (!parentBlockValidationResult.IsSuccess)
                {
                    return parentBlockValidationResult;
                }

                var parentBlock = _blockchainRepository.GetBlock(block.ParentUniqueId);
                if (parentBlock.Depth + 1 != mappedBlock.Depth)
                {
                    _statisticService.RegisterRejectedBlock();
                    return new ErrorResponse<bool>("The depth of incoming block is incorrect!", false);
                }

                var mappedParentBlock = LocalMapper.Map<BlockBase>(parentBlock);
                ((Model.Block.Block) mappedBlock).Parent = mappedParentBlock;
            }

            var validationResult = _blockchainValidator.Validate(mappedBlock);
            if (!validationResult.IsSuccess)
            {
                _statisticService.RegisterRejectedBlock();
                return new ErrorResponse<bool>("The validation for the block failed!", false, validationResult.Errors);
            }

            _blockchainRepository.AddBlock(incomingBlock);
            return new SuccessResponse<bool>("The block has been accepted and appended!", true);
        }

        private void DistributeBlock(DataAccess.Model.Block.BlockBase blockBase)
        {
            var blockJson = JsonConvert.SerializeObject(blockBase);
            var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(blockJson));
            DistributeBlock(new EncodedBlock
            {
                Id = Guid.NewGuid().ToString(),
                Base64Block = base64String,
                NodeSenderId = _nodeId,
                NodesAcceptedIds = new List<string>()
            });
        }

        private void DistributeBlock(EncodedBlock encodedBlock)
        {
            _queue.QueueBackgroundWorkItem(token => Task.Run(() =>
            {
                encodedBlock.NodeSenderId = _nodeId;
                encodedBlock.NodesAcceptedIds.Add(_nodeId);

                _serverNodes.Values.Where(node => !encodedBlock.NodesAcceptedIds.Contains(node.Id))
                    .ParallelForEach(node =>
                    {
                        Task.Run(async () =>
                        {
                            // The delay
                            await Task.Delay((int) node.Delay, token);
                            _httpService.Post($"{node.HttpAddress}/api/consensus", new JsonContent(encodedBlock));
                        }, token);
                    });
            }, token));
        }

        private BaseResponse<bool> ValidateTransactionsDuplicates(BlockBase blockBase)
        {
            var longestBlockchain = _blockchainRepository.GetLongestBlockchain();
            if (longestBlockchain != null)
            {
                var longestBlockchainTransactionsIds = longestBlockchain.Blocks.SelectMany(b => b.Body.Transactions)
                    .Select(t => t.Id).ToList();
                if (blockBase.Body.Transactions.Any(t => longestBlockchainTransactionsIds.Contains(t.Id)))
                {
                    return new ErrorResponse<bool>("One of the transactions already exists in main chain!", false);
                }
            }

            return new SuccessResponse<bool>("The transactions are valid!", true);
        }

        private BaseResponse<bool> ValidateParentBlock(Block block, string senderNodeId)
        {
            if (_blockchainRepository.BlockExists(block.ParentUniqueId))
            {
                return new SuccessResponse<bool>("The parent block exists!", true);
            }

            var nodeAddress = _serverNodes.Values.FirstOrDefault(n => n.Id == senderNodeId)?.HttpAddress;
            if (nodeAddress == null)
            {
                _statisticService.RegisterRejectedBlock();
                return new ErrorResponse<bool>($"The parent block with id: {block.ParentUniqueId} don't exist!", false);
            }

            var httpResponse = _httpService.Get($"{nodeAddress}/api/blockchain/{block.ParentUniqueId}");
            if (!httpResponse.IsSuccessStatusCode)
            {
                _statisticService.RegisterRejectedBlock();
                return new ErrorResponse<bool>($"The parent block with id: {block.ParentUniqueId} don't exist!", false);
            }

            var json = httpResponse.Content.ReadAsString();
            if (json != null)
            {
                var blockBase = BlockchainConverter.DeserializeBlock(json);
                return AcceptBlock(blockBase, senderNodeId);
            }

            _statisticService.RegisterRejectedBlock();
            return new ErrorResponse<bool>($"The parent block with id: {block.ParentUniqueId} don't exist!", false);
        }
    }
}