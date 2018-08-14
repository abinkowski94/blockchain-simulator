using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using BlockchainSimulator.BusinessLogic.Model.MappingProfiles;
using BlockchainSimulator.BusinessLogic.Queue;
using BlockchainSimulator.BusinessLogic.Validators;
using BlockchainSimulator.DataAccess.Converters.Specific;
using BlockchainSimulator.DataAccess.Model;
using BlockchainSimulator.DataAccess.Repositories;
using Newtonsoft.Json;

namespace BlockchainSimulator.BusinessLogic.Services.Specific
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

        public override bool AcceptBlockchain(string base64Blockchain)
        {
            if (base64Blockchain == null)
            {
                return false;
            }

            var blockchainJson = Encoding.UTF8.GetString(Convert.FromBase64String(base64Blockchain));
            var incomingBlockchain = JsonConvert.DeserializeObject<Blockchain>(blockchainJson,
                new JsonSerializerSettings
                    {Converters = new JsonConverter[] {new BlockConverter(), new NodeConverter()}});
            var currentBlockchain = _blockchainRepository.GetBlockchain();

            if (incomingBlockchain != null &&
                (currentBlockchain == null || incomingBlockchain.Blocks.Count > currentBlockchain.Blocks.Count))
            {
                var blockchainForValidation = LocalMapper.ManualMap(incomingBlockchain);
                if (_blockchainValidator.Validate(blockchainForValidation).IsSuccess)
                {
                    _blockchainRepository.SaveBlockchain(incomingBlockchain);
                    ReachConsensus();
                    return true;
                }
            }

            return false;
        }

        public override void ReachConsensus()
        {
            _serverNodes.Select(kv => kv.Value).ToList().ForEach(node =>
            {
                _queue.QueueBackgroundWorkItem(async token =>
                {
                    var blockchain = _blockchainRepository.GetBlockchain();
                    var blockchainJson = JsonConvert.SerializeObject(blockchain);
                    var encodedBlockchain = Convert.ToBase64String(Encoding.UTF8.GetBytes(blockchainJson));
                    var body = new {base64Blockchain = encodedBlockchain};

                    using (var httpClientHandler = new HttpClientHandler())
                    {
                        httpClientHandler.ServerCertificateCustomValidationCallback =
                            (message, cert, chain, errors) => true;
                        using (var httpClient = new HttpClient(httpClientHandler))
                        {
                            try
                            {
                                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8,
                                    "application/json");
                                await httpClient.PostAsync($"{node.HttpAddress}/api/consensus", content, token);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                    }
                });
            });
        }
    }
}