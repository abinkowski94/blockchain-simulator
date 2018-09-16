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
using System.Threading;
using System.Threading.Tasks;
using BlockchainSimulator.Common.Models.Consensus;
using BlockchainSimulator.Common.Models.WebClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Block = BlockchainSimulator.Node.DataAccess.Model.Block.Block;

namespace BlockchainSimulator.Node.BusinessLogic.Services.Specific
{
    public class ProofOfWorkConsensusService : BaseConsensusService
    {
        private readonly ConcurrentBag<string> _encodedBlocksIds;
        private readonly IBlockchainRepository _blockchainRepository;
        private readonly IBlockchainValidator _blockchainValidator;
        private readonly IStatisticService _statisticService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpService _httpService;
        private readonly string _nodeId;

        public ProofOfWorkConsensusService(IBackgroundTaskQueue queue, IBlockchainRepository blockchainRepository,
            IBlockchainValidator blockchainValidator, IHttpService httpService, IStatisticService statisticService,
            IConfiguration configuration, IServiceProvider serviceProvider) : base(queue)
        {
            _encodedBlocksIds = new ConcurrentBag<string>();
            _blockchainRepository = blockchainRepository;
            _blockchainValidator = blockchainValidator;
            _httpService = httpService;
            _statisticService = statisticService;
            _serviceProvider = serviceProvider;
            _nodeId = configuration["Node:Id"];
        }

        public override void AcceptBlocks(EncodedBlock encodedBlock)
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
                    _serviceProvider.GetService<ITransactionService>().ReMineTransactions();
                }

                return result;
            }, token));
        }

        public override void AcceptBlock(BlockBase blockBase)
        {
            _queue.QueueBackgroundWorkItem(token => new Task<BaseResponse<bool>>(() =>
            {
                if (blockBase == null)
                {
                    return new ErrorResponse<bool>("The block can not be null!", false);
                }

                var validationResult = ValidateDuplicates(blockBase);
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }

                var mappedBlock = LocalMapper.Map<DataAccess.Model.Block.BlockBase>(blockBase);
                _blockchainRepository.AddBlock(mappedBlock);
                DistributeBlock(mappedBlock);
                _serviceProvider.GetService<ITransactionService>().ReMineTransactions();

                return new SuccessResponse<bool>("The block has been accepted and appended!", true);
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

            var duplicatesValidationResult = ValidateDuplicates(mappedBlock);
            if (!duplicatesValidationResult.IsSuccess)
            {
                return duplicatesValidationResult;
            }

            if (incomingBlock is Block block)
            {
                var parentBlockValidationResult = ValidateParentBlock(senderNodeId, block);
                if (!parentBlockValidationResult.IsSuccess)
                {
                    return parentBlockValidationResult;
                }

                var parent = _blockchainRepository.GetBlock(block.ParentUniqueId);
                if (parent.Depth + 1 != mappedBlock.Depth)
                {
                    _statisticService.RegisterRejectedBlock();
                    return new ErrorResponse<bool>("The depth of incoming block is incorrect!", false);
                }

                var mappedParent = LocalMapper.Map<BlockBase>(parent);
                ((Model.Block.Block) mappedBlock).Parent = mappedParent;
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

        private BaseResponse<bool> ValidateParentBlock(string senderNodeId, Block block)
        {
            if (!_blockchainRepository.BlockExists(block.ParentUniqueId))
            {
                var nodeAddress = _serverNodes.Values.FirstOrDefault(n => n.Id == senderNodeId)?.HttpAddress;
                if (nodeAddress == null)
                {
                    _statisticService.RegisterRejectedBlock();
                    return new ErrorResponse<bool>(
                        $"The parent block with id: {block.ParentUniqueId} does not exist!", false);
                }

                var httpResponse = _httpService.Get($"{nodeAddress}/api/blockchain/{block.ParentUniqueId}");
                if (!httpResponse.IsSuccessStatusCode)
                {
                    _statisticService.RegisterRejectedBlock();
                    return new ErrorResponse<bool>($"The parent block with id: {block.ParentUniqueId} does not exist!",
                        false);
                }

                var jsonTask = httpResponse.Content.ReadAsStringAsync();
                jsonTask.Wait();
                var json = jsonTask.Result;
                var blockBase = BlockchainConverter.DeserializeBlock(json);

                if (blockBase != null)
                {
                    return AcceptBlock(blockBase, senderNodeId);
                }

                _statisticService.RegisterRejectedBlock();
                return new ErrorResponse<bool>($"The parent block with id: {block.ParentUniqueId} does not exist!",
                    false);
            }

            return new SuccessResponse<bool>("The block exists!", true);
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
            _queue.QueueBackgroundWorkItem(token => new Task(() =>
            {
                encodedBlock.NodeSenderId = _nodeId;
                encodedBlock.NodesAcceptedIds.Add(_nodeId);

                _serverNodes.Values.Where(node => !encodedBlock.NodesAcceptedIds.Contains(node.Id))
                    .ParallelForEach(node =>
                    {
                        // The delay
                        Thread.Sleep((int) node.Delay);
                        _httpService.Post($"{node.HttpAddress}/api/consensus", new JsonContent(encodedBlock));
                    });
            }, token));
        }

        private BaseResponse<bool> ValidateDuplicates(BlockBase blockBase)
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
    }
}