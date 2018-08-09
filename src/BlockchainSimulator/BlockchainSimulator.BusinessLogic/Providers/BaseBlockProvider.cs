using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.Transaction;
using BlockchainSimulator.BusinessLogic.Services;

namespace BlockchainSimulator.BusinessLogic.Providers
{
    public abstract class BaseBlockProvider : BaseService, IBlockProvider
    {
        private readonly IMerkleTreeProvider _merkleTreeProvider;
        protected readonly IEncryptionService _encryptionService;

        protected BaseBlockProvider(IMerkleTreeProvider merkleTreeProvider, IEncryptionService encryptionService)
        {
            _merkleTreeProvider = merkleTreeProvider;
            _encryptionService = encryptionService;
        }

        protected abstract BlockBase FillBlock(BlockBase currentBlock);

        public BlockBase CreateBlock(HashSet<Transaction> transactions, BlockBase parentBlock = null)
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
                ParentHash = _encryptionService.GetSha256Hash(parentBlock?.BlockJson),
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
                    Id = Convert.ToString(0, 16)
                };
            }
            else
            {
                newBlock = new Block
                {
                    Id = Convert.ToString(long.Parse(parentBlock.Id) + 1, 16),
                    ParentId = parentBlock.Id,
                    Parent = parentBlock
                };
            }

            newBlock.Header = header;
            newBlock.Body = body;

            return FillBlock(newBlock);
        }
    }
}