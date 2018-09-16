using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace BlockchainSimulator.Node.BusinessLogic.Providers
{
    public abstract class BaseBlockProvider : BaseService, IBlockProvider
    {
        private readonly IMerkleTreeProvider _merkleTreeProvider;
        private readonly string _nodeId;

        protected BaseBlockProvider(IMerkleTreeProvider merkleTreeProvider, IConfiguration configuration)
        {
            _merkleTreeProvider = merkleTreeProvider;
            _nodeId = configuration["Node:Id"];
        }

        protected abstract BlockBase FillBlock(BlockBase currentBlock);

        public BlockBase CreateBlock(HashSet<Transaction> transactions, DateTime enqueueTime,
            BlockBase parentBlock = null)
        {
            if (transactions == null)
            {
                return null;
            }

            if (!transactions.Any())
            {
                return null;
            }

            var tree = _merkleTreeProvider.GetMerkleTree(transactions);

            var header = new Header
            {
                ParentHash = EncryptionService.GetSha256Hash(parentBlock?.BlockJson),
                MerkleTreeRootHash = tree.Hash,
                TimeStamp = DateTime.UtcNow,
                Version = null,
                Nonce = null,
                Target = null
            };

            var body = new Body
            {
                MerkleTree = tree,
                Transactions = transactions
            };

            BlockBase newBlock;
            if (parentBlock == null)
            {
                newBlock = new GenesisBlock
                {
                    Id = Convert.ToString(0, 16),
                    UniqueId = $"{Guid.NewGuid()}-{_nodeId}",
                    QueueTime = DateTime.UtcNow - enqueueTime
                };
            }
            else
            {
                newBlock = new Block
                {
                    Id = Convert.ToString(Convert.ToInt32(parentBlock.Id, 16) + 1, 16),
                    UniqueId = $"{Guid.NewGuid()}-{_nodeId}",
                    ParentUniqueId = parentBlock.UniqueId,
                    QueueTime = DateTime.UtcNow - enqueueTime,
                    Depth = parentBlock.Depth + 1,
                    Parent = parentBlock
                };
            }

            newBlock.Header = header;
            newBlock.Body = body;

            return FillBlock(newBlock);
        }
    }
}