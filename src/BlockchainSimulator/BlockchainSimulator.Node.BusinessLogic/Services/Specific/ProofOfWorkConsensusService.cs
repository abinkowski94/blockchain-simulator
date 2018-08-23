using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using BlockchainSimulator.Node.BusinessLogic.Model.MappingProfiles;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Queues.BackgroundTasks;
using BlockchainSimulator.Node.BusinessLogic.Validators;
using BlockchainSimulator.Node.DataAccess.Converters;
using BlockchainSimulator.Node.DataAccess.Repositories;
using Newtonsoft.Json;

namespace BlockchainSimulator.Node.BusinessLogic.Services.Specific
{
    public class ProofOfWorkConsensusService : BaseConsensusService
    {
        private readonly IBlockchainRepository _blockchainRepository;
        private readonly IBlockchainValidator _blockchainValidator;

        public ProofOfWorkConsensusService(IBackgroundTaskQueue queue, IBlockchainRepository blockchainRepository,
            IBlockchainValidator blockchainValidator) : base(queue)
        {
            _blockchainRepository = blockchainRepository;
            _blockchainValidator = blockchainValidator;
        }

        public override BaseResponse<bool> AcceptBlockchain(string base64Blockchain)
        {
            if (base64Blockchain == null)
            {
                return new ErrorResponse<bool>("The blockchain can not be null!", false);
            }

            var blockchainJson = Encoding.UTF8.GetString(Convert.FromBase64String(base64Blockchain));
            var incomingBlockchain = BlockchainConverter.DeserializeBlockchain(blockchainJson);
            var currentBlockchain = _blockchainRepository.GetBlockchain();

            if (currentBlockchain != null && incomingBlockchain.Blocks.Count <= currentBlockchain.Blocks.Count)
            {
                return new ErrorResponse<bool>("The incoming blockchain is shorter than the current!", false);
            }

            var blockchainForValidation = LocalMapper.ManualMap(incomingBlockchain);
            var validationResult = _blockchainValidator.Validate(blockchainForValidation);
            if (!validationResult.IsSuccess)
            {
                return new ErrorResponse<bool>("The incoming blockchain is invalid!", false, validationResult.Errors);
            }

            _blockchainRepository.SaveBlockchain(incomingBlockchain);
            ReachConsensus();

            return new SuccessResponse<bool>("The blockchain has been accepted and swapped!", true);
        }

        public override void ReachConsensus()
        {
            _serverNodes.Select(kv => kv.Value).ToList().ForEach(node =>
            {
                _queue.QueueBackgroundWorkItem(async token =>
                {
                    // The delay
                    Thread.Sleep((int) node.Delay);
                    
                    var blockchain = _blockchainRepository.GetBlockchain();
                    var blockchainJson = JsonConvert.SerializeObject(blockchain);
                    var encodedBlockchain = Convert.ToBase64String(Encoding.UTF8.GetBytes(blockchainJson));
                    var body = JsonConvert.SerializeObject(new {base64Blockchain = encodedBlockchain});

                    try
                    {
                        using (var handler = new HttpClientHandler())
                        {
                            handler.ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true;
                            using (var httpClient = new HttpClient(handler))
                            {
                                var content = new StringContent(body, Encoding.UTF8, "application/json");
                                await httpClient.PostAsync($"{node.HttpAddress}/api/consensus", content, token);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // TODO: log errors
                        Console.WriteLine(e);
                    }
                });
            });
        }
    }
}