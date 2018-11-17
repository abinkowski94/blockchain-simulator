using BlockchainSimulator.Node.DataAccess.Converters;
using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Model.Block;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace BlockchainSimulator.Node.DataAccess.Repositories
{
    public class BlockchainRepository : IBlockchainRepository
    {
        private readonly IFileRepository _fileRepository;
        private readonly JsonSerializer _serializer;
        private readonly IMemoryCache _cache;

        private readonly string _blockchainFileName;
        private readonly object _padlock = new object();

        public BlockchainRepository(IFileRepository fileRepository, IMemoryCache cache, IConfiguration configuration)
        {
            _fileRepository = fileRepository;
            _serializer = new JsonSerializer {Converters = {new BlockConverter(), new NodeConverter()}};
            _cache = cache;
            _blockchainFileName = $"blockchainTree-{configuration["Node:Id"]}.json";           
        }

        public BlockBase GetBlock(string uniqueId)
        {
            return uniqueId == null
                ? null
                : _cache.GetOrCreate(CacheKeys.BlockId(uniqueId),
                    entry => GetBlockchainTree()?.Blocks?.FirstOrDefault(b => b.UniqueId == uniqueId));
        }

        public BlockBase GetLastBlock()
        {
            return _cache.GetOrCreate(CacheKeys.LastBlock,
                entry => GetBlockchainTree()?.Blocks?.OrderByDescending(b => b.Depth).ThenBy(b => b.Header.TimeStamp)
                    .FirstOrDefault());
        }

        public BlockchainTreeMetadata GetBlockchainMetadata()
        {
            return new BlockchainTreeMetadata
            {
                Nodes = GetBlockchainTree()?.Blocks?.Count ?? 0
            };
        }

        public BlockchainTree GetBlockchainFromBranch(string uniqueId)
        {
            if (uniqueId == null)
            {
                return null;
            }

            lock (_padlock)
            {
                var blockchainTree = GetBlockchainTree();
                if (blockchainTree?.Blocks == null)
                {
                    return null;
                }

                var blockchain = new BlockchainTree {Blocks = new List<BlockBase>()};
                BlockBase block = null;
                do
                {
                    block = block == null
                        ? blockchainTree.Blocks.FirstOrDefault(b => b.UniqueId == uniqueId)
                        : blockchainTree.Blocks.FirstOrDefault(b =>
                            block is Block current && b.UniqueId == current.ParentUniqueId);

                    if (block != null)
                    {
                        blockchain.Blocks.Add(block);
                    }
                } while (block != null);

                blockchain.Blocks = blockchain.Blocks.OrderBy(b => b.Depth).ToList();
                return blockchain;
            }
        }

        public BlockchainTree GetLongestBlockchain()
        {
            return _cache.GetOrCreate(CacheKeys.LongestBlockchain, entry =>
            {
                var blockchainTree = GetBlockchainTree();
                if (blockchainTree?.Blocks == null)
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
                            // ReSharper disable once AccessToModifiedClosure
                            block is Block current && b.UniqueId == current.ParentUniqueId);

                    if (block != null)
                    {
                        blockchain.Blocks.Add(block);
                    }
                } while (block != null);

                return blockchain;
            });
        }

        public BlockchainTree GetBlockchainTree()
        {
            var blocks = _cache.GetOrCreate(CacheKeys.BlockchainTree, entry =>
            {
                lock (_padlock)
                {
                    using (var stream = _fileRepository.GetFileReader(_blockchainFileName))
                    using (var reader = new JsonTextReader(stream))
                    {
                        return stream == StreamReader.Null ? null : BlockchainConverter.DeserializeBlockchain(reader);
                    }
                }
            })?.Blocks?.ToList();

            return new BlockchainTree {Blocks = blocks};
        }

        public bool BlockExists(string uniqueId)
        {
            return GetBlockchainTree()?.Blocks?.Any(b => b.UniqueId == uniqueId) ?? false;
        }

        public void AddBlock(BlockBase blockBase)
        {
            lock (_padlock)
            {
                var metaData = GetBlockchainMetadata();
                if (metaData.Nodes < 1)
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

        public void Clear()
        {
            lock (_padlock)
            {
                SaveBlockchain(new BlockchainTree {Blocks = new List<BlockBase>()});
            }
        }

        private void SaveBlockchain(BlockchainTree blockchainTree)
        {
            _cache.Remove(CacheKeys.LastBlock);
            _cache.Remove(CacheKeys.BlockchainTree);
            _cache.Remove(CacheKeys.LongestBlockchain);

            _cache.Set(CacheKeys.BlockchainTree, blockchainTree);

            using (var streamWriter = _fileRepository.GetFileWriter(_blockchainFileName))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                _serializer.Serialize(jsonWriter, blockchainTree);
                jsonWriter.Flush();
            }
        }
    }
}