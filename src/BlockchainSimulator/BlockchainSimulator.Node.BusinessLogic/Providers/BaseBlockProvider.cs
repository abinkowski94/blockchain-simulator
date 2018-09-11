using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockchainSimulator.Node.BusinessLogic.Providers
{
    public abstract class BaseBlockProvider : BaseService, IBlockProvider
    {
        private readonly IMerkleTreeProvider _merkleTreeProvider;

        protected BaseBlockProvider(IMerkleTreeProvider merkleTreeProvider)
        {
            _merkleTreeProvider = merkleTreeProvider;
        }

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
                    UniqueId = Guid.NewGuid().ToString(),
                    QueueTime = DateTime.UtcNow - enqueueTime
                };
            }
            else
            {
                newBlock = new Block
                {
                    Id = Convert.ToString(Convert.ToInt32(parentBlock.Id, 16) + 1, 16),
                    UniqueId = Guid.NewGuid().ToString(),
                    ParentUniqueId = parentBlock.UniqueId,
                    QueueTime = DateTime.UtcNow - enqueueTime,
                    Parent = parentBlock
                };
            }

            newBlock.Header = header;
            newBlock.Body = body;

            return FillBlock(newBlock);
        }

        protected abstract BlockBase FillBlock(BlockBase currentBlock);
    }
}