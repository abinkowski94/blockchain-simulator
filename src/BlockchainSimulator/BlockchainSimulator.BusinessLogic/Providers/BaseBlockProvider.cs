using System;
using System.Collections.Generic;
using System.Globalization;
using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.Transaction;
using BlockchainSimulator.BusinessLogic.Services;

namespace BlockchainSimulator.BusinessLogic.Providers
{
    public abstract class BaseBlockProvider : BaseService, IBlockProvider
    {
        private readonly IMerkleTreeProvider _merkleTreeProvider;
        private readonly IEncryptionService _encryptionService;

        protected BaseBlockProvider(IMerkleTreeProvider merkleTreeProvider, IEncryptionService encryptionService)
        {
            _merkleTreeProvider = merkleTreeProvider;
            _encryptionService = encryptionService;
        }

        protected abstract Block FillBlock(BlockBase currentBlock);

        public Block CreateBlock(HashSet<Transaction> transactions, Block parentBlock)
        {
            var tree = _merkleTreeProvider.GetMerkleTree(transactions);

            var header = new Header
            {
                ParentHash = parentBlock == null ? null : _encryptionService.GetSha256Hash(parentBlock.BlockJson),
                MerkleTreeRootHash = tree.Hash,
                TimeStamp = DateTime.UtcNow,
                Version = "",
                Nonce = "",
                Target = ""
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
                    Id = "1"
                };
            }
            else
            {
                newBlock = new Block
                {
                    Id = (decimal.Parse(parentBlock.Id) + 1).ToString(CultureInfo.InvariantCulture),
                    Parent = parentBlock
                };
            }

            newBlock.Header = header;
            newBlock.Body = body;

            return FillBlock(newBlock);
        }
    }
}