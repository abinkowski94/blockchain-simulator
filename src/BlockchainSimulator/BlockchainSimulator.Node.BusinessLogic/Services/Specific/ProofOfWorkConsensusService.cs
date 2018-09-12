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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Block = BlockchainSimulator.Node.DataAccess.Model.Block.Block;

namespace BlockchainSimulator.Node.BusinessLogic.Services.Specific
{
    public class ProofOfWorkConsensusService : BaseConsensusService
    {
        private readonly IBlockchainRepository _blockchainRepository;
        private readonly IBlockchainValidator _blockchainValidator;
        private readonly IStatisticService _statisticService;
        private readonly IHttpService _httpService;
        private readonly object _padlock = new object();

        public ProofOfWorkConsensusService(IBackgroundTaskQueue queue, IBlockchainRepository blockchainRepository,
            IBlockchainValidator blockchainValidator, IHttpService httpService,
            IStatisticService statisticService) : base(queue)
        {
            _blockchainRepository = blockchainRepository;
            _blockchainValidator = blockchainValidator;
            _httpService = httpService;
            _statisticService = statisticService;
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
                    return new ErrorResponse<bool>("The block already exists!", false);
                }

                if (incomingBlock is Block block && !_blockchainRepository.BlockExists(block.ParentUniqueId))
                {
                    return new ErrorResponse<bool>($"The parent block with id: {block.ParentUniqueId} does not exist!",
                        false);
                }

                var mappedBlock = LocalMapper.Map<BlockBase>(incomingBlock);
                var validationResult = _blockchainValidator.Validate(mappedBlock);

                if (!validationResult.IsSuccess)
                {
                    return new ErrorResponse<bool>("The validation for the block failed!", false,
                        validationResult.Errors);
                }

                _blockchainRepository.AddBlock(incomingBlock);
                ReachConsensus(incomingBlock);

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
    }
}