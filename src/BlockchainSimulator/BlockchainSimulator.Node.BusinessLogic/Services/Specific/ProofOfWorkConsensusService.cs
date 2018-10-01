using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Common.Models.Consensus;
using BlockchainSimulator.Common.Models.WebClient;
using BlockchainSimulator.Common.Services;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using BlockchainSimulator.Node.BusinessLogic.Validators;
using BlockchainSimulator.Node.DataAccess.Converters;
using BlockchainSimulator.Node.DataAccess.Repositories;
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
            Task.Run(() =>
            {
                if (encodedBlock?.Base64Block != null && !_encodedBlocksIds.Contains(encodedBlock.Id))
                {
                    var blockchainJson = Encoding.UTF8.GetString(Convert.FromBase64String(encodedBlock.Base64Block));
                    var incomingBlock = BlockchainConverter.DeserializeBlock(blockchainJson);

                    var result = AcceptBlock(incomingBlock, encodedBlock.NodeSenderId);
                    if (result.IsSuccess)
                    {
                        _encodedBlocksIds.Add(encodedBlock.Id);
                        DistributeBlock(encodedBlock);
                    }
                }
            });
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

        private BaseResponse<bool> AcceptBlock(DAM.Block.BlockBase incomingBlock, string senderNodeId)
        {
            if (_blockchainRepository.BlockExists(incomingBlock.UniqueId))
            {
                _statisticService.RegisterRejectedBlock();
                return new ErrorResponse<bool>("The block already exists!", false);
            }

            var mappedBlock = LocalMapper.Map<BlockBase>(incomingBlock);
            if (incomingBlock is DAM.Block.Block block)
            {
                var parentBlockValidationResult = ValidateParentBlock(block, senderNodeId);
                if (!parentBlockValidationResult.IsSuccess)
                {
                    Console.WriteLine(parentBlockValidationResult.Message);
                    Console.WriteLine(_nodeId);
                    return parentBlockValidationResult;
                }

                var parentBlock = _blockchainRepository.GetBlock(block.ParentUniqueId);
                if (parentBlock.Depth + 1 != mappedBlock.Depth)
                {
                    _statisticService.RegisterRejectedBlock();
                    return new ErrorResponse<bool>("The depth of incoming block is incorrect!", false);
                }

                var mappedParentBlock = LocalMapper.Map<BlockBase>(parentBlock);
                ((Block)mappedBlock).Parent = mappedParentBlock;
            }

            var duplicatesValidationResult = ValidateTransactionsDuplicates(mappedBlock);
            if (!duplicatesValidationResult.IsSuccess)
            {
                return duplicatesValidationResult;
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

        private void DistributeBlock(DAM.Block.BlockBase blockBase)
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
            Task.Run(() =>
            {
                encodedBlock.NodeSenderId = _nodeId;
                encodedBlock.NodesAcceptedIds.Add(_nodeId);

                ServerNodes.Values.Where(node => !encodedBlock.NodesAcceptedIds.Contains(node.Id))
                    .ParallelForEach(node =>
                    {
                        Task.Run(() =>
                        {
                            // The delay
                            Task.Delay((int)node.Delay).Wait();
                            HttpService.Post($"{node.HttpAddress}/api/consensus", new JsonContent(encodedBlock));
                        });
                    });
            });
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

            var nodeAddress = ServerNodes.Values.FirstOrDefault(n => n.Id == senderNodeId)?.HttpAddress;
            if (nodeAddress == null)
            {
                _statisticService.RegisterRejectedBlock();
                return new ErrorResponse<bool>($"The parent block with id: {block.ParentUniqueId} don't exist!", false);
            }

            var httpResponse = HttpService.Get($"{nodeAddress}/api/blockchain/{block.ParentUniqueId}");
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