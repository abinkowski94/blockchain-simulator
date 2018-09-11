using System.IO;
using System.Linq;
using BlockchainSimulator.Node.DataAccess.Converters;
using BlockchainSimulator.Node.DataAccess.Converters.Specific;
using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Model.Block;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlockchainSimulator.Node.DataAccess.Repositories
{
    public class BlockchainRepository : IBlockchainRepository
    {
        private readonly string _blockchainFileName;
        private readonly IFileRepository _fileRepository;
        private readonly object _padlock = new object();

        public BlockchainRepository(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
            _blockchainFileName = "blockchain.json";
        }

        public Blockchain GetBlockchain()
        {
            lock (_padlock)
            {
                using (var streamReader = _fileRepository.GetFileReader(_blockchainFileName))
                using (var reader = new JsonTextReader(streamReader))
                {
                    return streamReader == StreamReader.Null ? null : BlockchainConverter.DeserializeBlockchain(reader);
                }
            }
        }

        public BlockBase GetLastBlock()
        {
            lock (_padlock)
            {
                using (var streamReader = _fileRepository.GetFileReader(_blockchainFileName))
                using (var reader = new JsonTextReader(streamReader))
                {
                    if (streamReader == StreamReader.Null)
                    {
                        return null;
                    }

                    var jObject = JObject.Load(reader);
                    return jObject.First.Last.Last.ToObject<BlockBase>(new JsonSerializer
                        {Converters = {new BlockConverter(), new NodeConverter()}});
                }
            }
        }

        public BlockBase GetBlock(string id)
        {
            lock (_padlock)
            {
                using (var streamReader = _fileRepository.GetFileReader(_blockchainFileName))
                using (var reader = new JsonTextReader(streamReader))
                {
                    if (streamReader == StreamReader.Null)
                    {
                        return null;
                    }

                    var jObject = JObject.Load(reader);
                    var jArray = (JArray) jObject.First.Last;
                    var block = jArray.FirstOrDefault(b => b.Value<string>("id") == id);
                    return block?.ToObject<BlockBase>(new JsonSerializer
                        {Converters = {new BlockConverter(), new NodeConverter()}});
                }
            }
        }

        public BlockchainMetadata GetBlockchainMetadata()
        {
            lock (_padlock)
            {
                using (var streamReader = _fileRepository.GetFileReader(_blockchainFileName))
                using (var reader = new JsonTextReader(streamReader))
                {
                    if (streamReader == StreamReader.Null)
                    {
                        return null;
                    }

                    var jObject = JObject.Load(reader);
                    return new BlockchainMetadata
                    {
                        Length = ((JArray) jObject.First.Last).Count
                    };
                }
            }
        }

        public void SaveBlockchain(Blockchain blockchain)
        {
            lock (_padlock)
            {
                using (var streamWriter = _fileRepository.GetFileWriter(_blockchainFileName))
                using (var jsonWriter = new JsonTextWriter(streamWriter))
                {
                    var jsonSerializer = new JsonSerializer();
                    jsonSerializer.Serialize(jsonWriter, blockchain);
                    jsonWriter.Flush();
                }
            }
        }
    }
}