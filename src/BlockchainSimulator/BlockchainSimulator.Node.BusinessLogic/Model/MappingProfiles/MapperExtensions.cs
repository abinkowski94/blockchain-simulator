using AutoMapper;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.DataAccess.Model;
using System.Collections.Generic;
using System.Linq;

namespace BlockchainSimulator.Node.BusinessLogic.Model.MappingProfiles
{
    public static class MapperExtensions
    {
        public static BlockBase ManualMap(this IMapper mapper, BlockchainTree blockchainTree)
        {
            if (blockchainTree == null)
            {
                return null;
            }

            var blocks = mapper.Map<List<BlockBase>>(blockchainTree.Blocks);

            blocks.Where(b => !b.IsGenesis).Cast<Block.Block>().ToList()
                .ForEach(b => b.Parent = blocks.FirstOrDefault(bl => b.ParentUniqueId == bl.UniqueId));

            blocks.ForEach(b => SetupTransactions(b.Body.MerkleTree, b.Body.Transactions));

            return blocks.OrderByDescending(b => (b as Block.Block)?.Depth ?? 0).FirstOrDefault();
        }

        public static BlockchainTree ManualMap(this IMapper mapper, BlockBase blockBase)
        {
            return GetBlockchain(mapper, blockBase);
        }

        private static BlockchainTree GetBlockchain(IMapper mapper, BlockBase blockBase, List<BlockBase> blocks = null)
        {
            if (blockBase == null)
            {
                return null;
            }

            if (blocks == null)
            {
                blocks = new List<BlockBase>();
            }

            switch (blockBase)
            {
                case Block.Block block:
                    blocks.Insert(0, block);
                    GetBlockchain(mapper, block.Parent, blocks);
                    break;

                case GenesisBlock genesisBlock:
                    blocks.Insert(0, genesisBlock);
                    break;
            }

            var mappedBlocks = mapper.Map<List<DataAccess.Model.Block.BlockBase>>(blocks);
            return new BlockchainTree { Blocks = mappedBlocks };
        }

        private static void SetupTransactions(MerkleNode merkleNode, HashSet<Transaction.Transaction> transactions)
        {
            if (merkleNode is Leaf leaf)
            {
                leaf.Transaction = transactions.FirstOrDefault(t => t.Id == leaf.TransactionId);
            }
            else if (merkleNode is Transaction.Node node)
            {
                SetupTransactions(node.LeftNode, transactions);
                SetupTransactions(node.RightNode, transactions);
            }
        }
    }
}