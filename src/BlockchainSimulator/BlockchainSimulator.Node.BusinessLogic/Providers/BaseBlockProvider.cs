using BlockchainSimulator.Common.Models;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Providers
{
    public abstract class BaseBlockProvider : BaseService, IBlockProvider
    {
        private readonly IConfigurationService _configurationService;
        private readonly IMerkleTreeProvider _merkleTreeProvider;

        protected BlockchainNodeConfiguration BlockchainNodeConfiguration => _configurationService.GetConfiguration();

        protected BaseBlockProvider(IMerkleTreeProvider merkleTreeProvider, IConfigurationService configurationService)
        {
            _configurationService = configurationService;
            _merkleTreeProvider = merkleTreeProvider;
        }

        public async Task<BlockBase> CreateBlock(HashSet<Transaction> transactions, DateTime enqueueTime,
            BlockBase parentBlock = null, CancellationToken token = default(CancellationToken))
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
                    UniqueId = $"{Guid.NewGuid()}-{BlockchainNodeConfiguration.NodeId}",
                    QueueTime = DateTime.UtcNow - enqueueTime
                };
            }
            else
            {
                newBlock = new Block
                {
                    Id = Convert.ToString(Convert.ToInt32(parentBlock.Id, 16) + 1, 16),
                    UniqueId = $"{Guid.NewGuid()}-{BlockchainNodeConfiguration.NodeId}",
                    ParentUniqueId = parentBlock.UniqueId,
                    QueueTime = DateTime.UtcNow - enqueueTime,
                    Depth = parentBlock.Depth + 1,
                    Parent = parentBlock
                };
            }

            newBlock.Header = header;
            newBlock.Body = body;

            return await FillBlock(newBlock, token);
        }

        protected abstract Task<BlockBase> FillBlock(BlockBase currentBlock, CancellationToken token);
    }
}