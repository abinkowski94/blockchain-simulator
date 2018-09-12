using System.Collections.Generic;
using System.Data;
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
            _blockchainFileName = "blockchainTree.json";
        }

        public BlockchainTree GetBlockchainTree()
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
                    var jArray = (JArray) jObject.First.Last;
                    var block = jArray.OrderByDescending(b => b.Value<int>("depth")).FirstOrDefault();
                    return block?.ToObject<BlockBase>(new JsonSerializer
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
                    var block = jArray.FirstOrDefault(b => b.Value<string>("uniqueId") == id);
                    return block?.ToObject<BlockBase>(new JsonSerializer
                        {Converters = {new BlockConverter(), new NodeConverter()}});
                }
            }
        }

        public BlockchainTreeMetadata GetBlockchainMetadata()
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
                    return new BlockchainTreeMetadata
                    {
                        Nodes = ((JArray) jObject.First.Last).Count
                    };
                }
            }
        }

        public void AddBlock(BlockBase blockBase)
        {
            lock (_padlock)
            {
                var metaData = GetBlockchainMetadata();
                if (metaData == null || metaData.Nodes < 1)
                {
                    if (!blockBase.IsGenesis)
                    {
                        throw new DataException("The blockchain tree is empty and the provided block is not genesis!");
                    }

                    SaveBlockchain(new BlockchainTree {Blocks = new List<BlockBase> {blockBase}});
                }
                else
                {
                    var blockchain = GetBlockchainTree();
                    blockchain.Blocks.Add(blockBase);
                    SaveBlockchain(blockchain);
                }
            }
        }

        public bool BlockExists(string uniqueId)
        {
            lock (_padlock)
            {
                using (var streamReader = _fileRepository.GetFileReader(_blockchainFileName))
                using (var reader = new JsonTextReader(streamReader))
                {
                    if (streamReader == StreamReader.Null)
                    {
                        return false;
                    }

                    var jObject = JObject.Load(reader);
                    var jArray = (JArray) jObject.First.Last;
                    return jArray.Any(b => b.Value<string>("uniqueId") == uniqueId);
                }
            }
        }

        public void SaveBlockchain(BlockchainTree blockchainTree)
        {
            lock (_padlock)
            {
                using (var streamWriter = _fileRepository.GetFileWriter(_blockchainFileName))
                using (var jsonWriter = new JsonTextWriter(streamWriter))
                {
                    var jsonSerializer = new JsonSerializer();
                    jsonSerializer.Serialize(jsonWriter, blockchainTree);
                    jsonWriter.Flush();
                }
            }
        }
    }
}