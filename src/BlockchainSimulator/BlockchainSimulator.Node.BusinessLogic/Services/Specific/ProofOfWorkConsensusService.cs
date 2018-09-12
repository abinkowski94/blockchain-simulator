using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Common.Models.WebClient;
using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Common.Services;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.MappingProfiles;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Validators;
using BlockchainSimulator.Node.DataAccess.Converters;
using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Repositories;
using Newtonsoft.Json;
using System;
using System.Linq;
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
        private readonly object _padlock = new object();
        private readonly IServiceProvider _serviceProvider;
        private readonly IStatisticService _statisticService;

        public ProofOfWorkConsensusService(IBackgroundTaskQueue queue, IBlockchainRepository blockchainRepository,
            IBlockchainValidator blockchainValidator, IHttpService httpService, IStatisticService statisticService,
            IServiceProvider serviceProvider) : base(queue)
        {
            _blockchainRepository = blockchainRepository;
            _blockchainValidator = blockchainValidator;
            _httpService = httpService;
            _statisticService = statisticService;
            _serviceProvider = serviceProvider;
        }

        public override BaseResponse<bool> AcceptBlockchain(string base64Blockchain)
        {
            if (base64Blockchain == null)
            {
                _statisticService.RegisterRejectedBlockchain();
                return new ErrorResponse<bool>("The blockchainTree can not be null!", false);
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
                return new ErrorResponse<bool>("The blockchainTree can not be null!", false);
            }

            var incomingBlockchain = LocalMapper.ManualMap(blockBase);
            return AcceptBlockchain(incomingBlockchain);
        }

        public override void ReachConsensus()
        {
            _serverNodes.Values.ForEach(node =>
            {
                _queue.QueueBackgroundWorkItem(token => Task.Run(() =>
                {
                    // The delay
                    Thread.Sleep((int) node.Delay);

                    var blockchain = _blockchainRepository.GetBlockchainTree();
                    var blockchainJson = JsonConvert.SerializeObject(blockchain);
                    var encodedBlockchain = Convert.ToBase64String(Encoding.UTF8.GetBytes(blockchainJson));
                    var content = new JsonContent(new {base64Blockchain = encodedBlockchain});

                    _httpService.Post($"{node.HttpAddress}/api/consensus", content, TimeSpan.FromSeconds(10), token);
                }, token));
            });
        }

        private BaseResponse<bool> AcceptBlockchain(BlockchainTree incomingBlockchainTree)
        {
            lock (_padlock)
            {
                var currentBlockchain = _blockchainRepository.GetBlockchainTree();
                if (currentBlockchain != null && incomingBlockchainTree.Blocks.Count <= currentBlockchain.Blocks.Count)
                {
                    return new ErrorResponse<bool>("The incoming blockchainTree is shorter than the current!", false);
                }

                var blockchainForValidation = LocalMapper.ManualMap(incomingBlockchainTree);
                var validationResult = _blockchainValidator.Validate(blockchainForValidation);
                if (!validationResult.IsSuccess)
                {
                    return new ErrorResponse<bool>("The incoming blockchainTree is invalid!", false,
                        validationResult.Errors);
                }

                _statisticService.AddBlockchainBranch(currentBlockchain);
                _statisticService.AddBlockchainBranch(incomingBlockchainTree);

                _blockchainRepository.SaveBlockchain(incomingBlockchainTree);

                ReachConsensus();
                CompareAndReMineBlocks(currentBlockchain, incomingBlockchainTree);

                return new SuccessResponse<bool>("The blockchainTree has been accepted and swapped!", true);
            }
        }

        private void CompareAndReMineBlocks(BlockchainTree currentBlockchainTree, BlockchainTree incomingBlockchainTree)
        {
            if (currentBlockchainTree?.Blocks?.Any() == true && incomingBlockchainTree?.Blocks?.Any() == true)
            {
                var transactionService =
                    (ITransactionService) _serviceProvider.GetService(typeof(ITransactionService));
                var incomingTransactionIds = incomingBlockchainTree.Blocks.SelectMany(b => b.Body.Transactions)
                    .Select(t => t.Id).ToArray();

                currentBlockchainTree.Blocks.SelectMany(b => b.Body.Transactions)
                    .Where(t => !incomingTransactionIds.Contains(t.Id)).ForEach(t =>
                    {
                        var mappedTransaction = LocalMapper.Map<Transaction>(t);
                        transactionService.AddTransaction(mappedTransaction);
                    });
            }
        }
    }
}