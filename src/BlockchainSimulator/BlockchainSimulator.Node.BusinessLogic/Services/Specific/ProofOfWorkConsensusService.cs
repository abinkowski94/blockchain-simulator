using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Common.Models.WebClient;
using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Common.Services;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Validators;
using BlockchainSimulator.Node.DataAccess.Converters;
using BlockchainSimulator.Node.DataAccess.Repositories;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using Microsoft.Extensions.Configuration;
using Block = BlockchainSimulator.Node.DataAccess.Model.Block.Block;

namespace BlockchainSimulator.Node.BusinessLogic.Services.Specific
{
    public class ProofOfWorkConsensusService : BaseConsensusService
    {
        private readonly IBlockchainRepository _blockchainRepository;
        private readonly IBlockchainValidator _blockchainValidator;
        private readonly IStatisticService _statisticService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly IHttpService _httpService;
        private readonly object _padlock = new object();

        public ProofOfWorkConsensusService(IBackgroundTaskQueue queue, IBlockchainRepository blockchainRepository,
            IBlockchainValidator blockchainValidator, IHttpService httpService, IStatisticService statisticService,
            IServiceProvider serviceProvider, IConfiguration configuration) : base(queue)
        {
            _blockchainRepository = blockchainRepository;
            _blockchainValidator = blockchainValidator;
            _httpService = httpService;
            _statisticService = statisticService;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public override BaseResponse<bool> AcceptBlock(string base64Block)
        {
            lock (_padlock)
            {
                if (base64Block == null)
                {
                    _statisticService.RegisterRejectedBlock();
                    return new ErrorResponse<bool>("The block can not be null!", false);
                }

                var blockchainJson = Encoding.UTF8.GetString(Convert.FromBase64String(base64Block));
                var incomingBlock = BlockchainConverter.DeserializeBlock(blockchainJson);

                if (_blockchainRepository.BlockExists(incomingBlock.UniqueId))
                {
                    _statisticService.RegisterRejectedBlock();
                    return new ErrorResponse<bool>("The block already exists!", false);
                }

                var mappedBlock = LocalMapper.Map<BlockBase>(incomingBlock);

                if (incomingBlock is Block block)
                {
                    if (!_blockchainRepository.BlockExists(block.ParentUniqueId))
                    {
                        _statisticService.RegisterRejectedBlock();
                        return new ErrorResponse<bool>(
                            $"The parent block with id: {block.ParentUniqueId} does not exist!", false);
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
                    return new ErrorResponse<bool>("The validation for the block failed!", false,
                        validationResult.Errors);
                }

                _blockchainRepository.AddBlock(incomingBlock);
                ReachConsensus(incomingBlock);
                //ReMineTransactions();

                return new SuccessResponse<bool>("The block has been accepted and appended!", true);
            }
        }

        public override BaseResponse<bool> AcceptBlock(BlockBase blockBase)
        {
            lock (_padlock)
            {
                if (blockBase == null)
                {
                    return new ErrorResponse<bool>("The block can not be null!", false);
                }

                var mappedBlock = LocalMapper.Map<DataAccess.Model.Block.BlockBase>(blockBase);

                _blockchainRepository.AddBlock(mappedBlock);
                ReachConsensus(mappedBlock);

                return new SuccessResponse<bool>("The block has been accepted and appended!", true);
            }
        }

        public override void ReachConsensus(DataAccess.Model.Block.BlockBase block)
        {
            _serverNodes.Values.ParallelForEach(node =>
            {
                _queue.QueueBackgroundWorkItem(token => Task.Run(() =>
                {
                    // The delay
                    Thread.Sleep((int) node.Delay);

                    var blockJson = JsonConvert.SerializeObject(block);
                    var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(blockJson));
                    var content = new JsonContent(new {base64Block = base64String});

                    _httpService.Post($"{node.HttpAddress}/api/consensus", content, TimeSpan.FromSeconds(10), token);
                }, token));
            });
        }

        private void ReMineTransactions()
        {
            lock (_padlock)
            {
                if (!(_serviceProvider.GetService(typeof(ITransactionService)) is ITransactionService transactionService
                    ))
                {
                    throw new ApplicationException("The transaction service has not been registered!");
                }

                var blockchainTreeTransactions = _blockchainRepository.GetBlockchainTree().Blocks
                    .SelectMany(b => b.Body.Transactions).Where(t => t.Id.EndsWith(_configuration["Node:Id"])).ToList();
                var longestBlockchainTransactionsIds = _blockchainRepository.GetLongestBlockchain().Blocks
                    .SelectMany(b => b.Body.Transactions).Where(t => t.Id.EndsWith(_configuration["Node:Id"]))
                    .Select(t => t.Id).ToList();

                blockchainTreeTransactions.Where(t => !longestBlockchainTransactionsIds.Contains(t.Id))
                    .ForEach(t =>
                    {
                        var mappedTransaction = LocalMapper.Map<Transaction>(t);
                        transactionService.AddTransaction(mappedTransaction);
                    });
            }
        }
    }
}