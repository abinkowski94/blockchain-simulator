using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Common.Services;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.MappingProfiles;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Validators;
using BlockchainSimulator.Node.DataAccess.Converters;
using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Repositories;
using Newtonsoft.Json;
using System;
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
        private readonly object _padlock = new object();

        public ProofOfWorkConsensusService(IBackgroundTaskQueue queue, IBlockchainRepository blockchainRepository,
            IBlockchainValidator blockchainValidator, IHttpService httpService) : base(queue)
        {
            _blockchainRepository = blockchainRepository;
            _blockchainValidator = blockchainValidator;
            _httpService = httpService;
        }

        public override BaseResponse<bool> AcceptBlockchain(string base64Blockchain)
        {
            if (base64Blockchain == null)
            {
                return new ErrorResponse<bool>("The blockchain can not be null!", false);
            }

            var blockchainJson = Encoding.UTF8.GetString(Convert.FromBase64String(base64Blockchain));
            var incomingBlockchain = BlockchainConverter.DeserializeBlockchain(blockchainJson);

            return AcceptBlockchain(incomingBlockchain);
        }

        public override BaseResponse<bool> AcceptBlockchain(BlockBase blockBase)
        {
            if (blockBase == null)
            {
                return new ErrorResponse<bool>("The blockchain can not be null!", false);
            }

            var incomingBlockchain = LocalMapper.ManualMap(blockBase);
            return AcceptBlockchain(incomingBlockchain);
        }

        public override void ReachConsensus()
        {
            _serverNodes.Select(kv => kv.Value).ForEach(node =>
            {
                _queue.QueueBackgroundWorkItem(token => Task.Run(() =>
                {
                    // The delay
                    Thread.Sleep((int) node.Delay);

                    var blockchain = _blockchainRepository.GetBlockchain();
                    var blockchainJson = JsonConvert.SerializeObject(blockchain);
                    var encodedBlockchain = Convert.ToBase64String(Encoding.UTF8.GetBytes(blockchainJson));
                    var body = JsonConvert.SerializeObject(new {base64Blockchain = encodedBlockchain});
                    var content = new StringContent(body, Encoding.UTF8, "application/json");

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

                _blockchainRepository.SaveBlockchain(incomingBlockchain);
                ReachConsensus();

                return new SuccessResponse<bool>("The blockchain has been accepted and swapped!", true);
            }
        }
    }
}