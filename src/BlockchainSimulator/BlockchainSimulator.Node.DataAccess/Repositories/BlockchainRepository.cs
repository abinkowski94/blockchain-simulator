using System;
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
        private readonly IFileRepository _fileRepository;
        private readonly JsonSerializer _serializer;
        private readonly string _blockchainFileName;
        private readonly object _padlock = new object();

        public BlockchainRepository(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
            _serializer = new JsonSerializer {Converters = {new BlockConverter(), new NodeConverter()}};
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

        public BlockchainTree GetLongestBlockchain()
        {
            lock (_padlock)
            {
                var blockchainTree = GetBlockchainTree();
                if (blockchainTree == null)
                {
                    return null;
                }

                var blockchain = new BlockchainTree {Blocks = new List<BlockBase>()};
                BlockBase block = null;
                do
                {
                    block = block == null
                        ? blockchainTree.Blocks.OrderByDescending(b => b.Depth).FirstOrDefault()
                        : blockchainTree.Blocks.FirstOrDefault(b =>
                            block is Block current && b.UniqueId == current.ParentUniqueId);

                    if (block != null)
                    {
                        blockchain.Blocks.Add(block);
                    }
                } while (block != null);

                return blockchain;
            }
        }

        public List<string> GetBlocksIds()
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
                    return jArray.Select(b => b.Value<string>("uniqueId")).ToList();
                }
            }
        }

        public List<BlockBase> GetBlocks(List<string> ids)
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
                    var blocks = jArray.Where(b => ids.Contains(b.Value<string>("uniqueId")));
                    return blocks.Select(b => b.ToObject<BlockBase>(_serializer)).ToList();
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
                    var block = jArray.OrderByDescending(b => b.Value<int>("depth"))
                        .ThenBy(b => b["header"].Value<DateTime>("timeStamp")).FirstOrDefault();
                    return block?.ToObject<BlockBase>(_serializer);
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
                    return block?.ToObject<BlockBase>(_serializer);
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
                    _serializer.Serialize(jsonWriter, blockchainTree);
                    jsonWriter.Flush();
                }
            }
        }
    }
}