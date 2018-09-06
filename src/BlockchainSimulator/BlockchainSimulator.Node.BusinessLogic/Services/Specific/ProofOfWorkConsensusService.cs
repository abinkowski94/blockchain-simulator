using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Common.Models.Http;
using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Common.Services;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Consensus;
using BlockchainSimulator.Node.BusinessLogic.Model.MappingProfiles;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using BlockchainSimulator.Node.BusinessLogic.Validators;
using BlockchainSimulator.Node.DataAccess.Converters;
using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Services.Specific
{
    public class ProofOfWorkConsensusService : BaseConsensusService
    {
        private readonly IBlockchainRepository _blockchainRepository;
        private readonly IBlockchainValidator _blockchainValidator;
        private readonly IHttpService _httpService;
        private readonly IMiningQueue _miningQueue;
        private readonly object _padlock = new object();
        private readonly IServiceProvider _serviceProvider;
        private readonly IStatisticService _statisticService;

        public ProofOfWorkConsensusService(IBackgroundTaskQueue queue, IBlockchainRepository blockchainRepository,
            IBlockchainValidator blockchainValidator, IHttpService httpService, IMiningQueue miningQueue,
            IStatisticService statisticService, IServiceProvider serviceProvider) : base(queue)
        {
            _blockchainRepository = blockchainRepository;
            _blockchainValidator = blockchainValidator;
            _httpService = httpService;
            _statisticService = statisticService;
            _miningQueue = miningQueue;
            _serviceProvider = serviceProvider;
        }

        public override BaseResponse<bool> AcceptBlockchain(string base64Blockchain)
        {
            if (base64Blockchain == null)
            {
                _statisticService.RegisterRejectedBlockchain();
                return new ErrorResponse<bool>("The blockchain can not be null!", false);
            }

            var blockchainJson = Encoding.UTF8.GetString(Convert.FromBase64String(base64Blockchain));
            var incomingBlockchain = BlockchainConverter.DeserializeBlockchain(blockchainJson);

            var result = AcceptBlockchain(incomingBlockchain);
            if (!result.IsSuccess)
            {
                _statisticService.RegisterRejectedBlockchain();
            }

            return result;
        }

        public override BaseResponse<bool> AcceptBlockchain(BlockBase blockBase)
        {
            if (blockBase == null)
            {
                return new ErrorResponse<bool>("The blockchain can not be null!", false);
            }

            var incomingBlockchain = LocalMapper.ManualMap(blockBase);

            var checkOtherNodesResponse = CheckOtherNodes(incomingBlockchain.Blocks.Count);
            if (!checkOtherNodesResponse.IsSuccess)
            {
                return checkOtherNodesResponse;
            }

            return AcceptBlockchain(incomingBlockchain);
        }

        public override void ReachConsensus()
        {
            _serverNodes.Select(kv => kv.Value).ForEach(node =>
            {
                _queue.QueueBackgroundWorkItem(token => Task.Run(() =>
                {
                    // The delay
                    Thread.Sleep((int)node.Delay);

                    var blockchain = _blockchainRepository.GetBlockchain();
                    var blockchainJson = JsonConvert.SerializeObject(blockchain);
                    var encodedBlockchain = Convert.ToBase64String(Encoding.UTF8.GetBytes(blockchainJson));
                    var content = new JsonContent(new { base64Blockchain = encodedBlockchain });

                    _httpService.Post($"{node.HttpAddress}/api/consensus", content, TimeSpan.FromSeconds(10), token);
                }, token));
            });
        }

        private BaseResponse<bool> AcceptBlockchain(Blockchain incomingBlockchain)
        {
            lock (_padlock)
            {
                var currentBlockchain = _blockchainRepository.GetBlockchain();
                if (currentBlockchain != null && incomingBlockchain.Blocks.Count <= currentBlockchain.Blocks.Count)
                {
                    return new ErrorResponse<bool>("The incoming blockchain is shorter than the current!", false);
                }

                var blockchainForValidation = LocalMapper.ManualMap(incomingBlockchain);
                var validationResult = _blockchainValidator.Validate(blockchainForValidation);
                if (!validationResult.IsSuccess)
                {
                    return new ErrorResponse<bool>("The incoming blockchain is invalid!", false,
                        validationResult.Errors);
                }

                _statisticService.AddBlockchainBranch(currentBlockchain);
                _statisticService.AddBlockchainBranch(incomingBlockchain);

                _blockchainRepository.SaveBlockchain(incomingBlockchain);

                //CompareAndRemineBlocks(currentBlockchain, incomingBlockchain);
                ReachConsensus();

                return new SuccessResponse<bool>("The blockchain has been accepted and swapped!", true);
            }
        }

        private BaseResponse<bool> CheckOtherNodes(int currentLength)
        {
            BaseResponse<bool> result = new ErrorResponse<bool>("The other longer blockchain was found!", false);

            _serverNodes.Select(kv => kv.Value).Select(n => new { Node = n, BlockchainLength = GetBlockchainLength(n) })
                .OrderByDescending(nl => nl.BlockchainLength).ForEach(nl =>
                {
                    if (currentLength < nl.BlockchainLength)
                    {
                        var content = _httpService.Get($"{nl.Node.HttpAddress}/api/blockchain").Content;
                        var blockchainTask = content.ReadAsStringAsync();
                        blockchainTask.Wait();

                        var incomingBlockchain = BlockchainConverter.DeserializeBlockchain(blockchainTask.Result);

                        var response = AcceptBlockchain(incomingBlockchain);
                        if (response.IsSuccess)
                        {
                            currentLength = incomingBlockchain.Blocks.Count;
                            result = new ErrorResponse<bool>("The other longer blockchain was found!", false);
                        }
                    }
                    else
                    {
                        result = new SuccessResponse<bool>("The current blockchain is the longest", true);
                    }
                });

            return result;
        }

        private void CompareAndRemineBlocks(Blockchain currentBlockchain, Blockchain incomingBlockchain)
        {
            if (currentBlockchain?.Blocks?.Any() == true && incomingBlockchain?.Blocks?.Any() == true)
            {
                var targetIndex = currentBlockchain.Blocks.Count;
                for (var index = 0; index < incomingBlockchain.Blocks.Count; index++)
                {
                    if (index < currentBlockchain.Blocks.Count)
                    {
                        var currentBlock = currentBlockchain.Blocks[index];
                        var incomingBlock = incomingBlockchain.Blocks[index];
                        if (currentBlock.Header.Nonce != incomingBlock.Header.Nonce
                            || currentBlock.Header.MerkleTreeRootHash != incomingBlock.Header.MerkleTreeRootHash
                            || currentBlock.Header.TimeStamp != incomingBlock.Header.TimeStamp)
                        {
                            targetIndex = index;
                            break;
                        }
                    }
                }

                var miningService = (IMiningService)_serviceProvider.GetService(typeof(IMiningService));

                currentBlockchain.Blocks.Skip(targetIndex).ForEach(b =>
                {
                    _miningQueue.QueueMiningTask(token => new Task(() =>
                    {
                        Console.WriteLine("Re-mine");
                        var transactions = LocalMapper.Map<HashSet<Transaction>>(b.Body.Transactions);
                        miningService.MineBlock(transactions, DateTime.UtcNow, token);
                    }, token));
                });
            }
        }

        private long GetBlockchainLength(ServerNode node)
        {
            var httpResponse = _httpService.Get($"{node.HttpAddress}/api/blockchain/meta-data");
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content;
                var contentTask = content.ReadAsAsync<BlockchainMetadata>();
                contentTask.Wait();

                return contentTask.Result.Length;
            }
            return 0;
        }
    }
}