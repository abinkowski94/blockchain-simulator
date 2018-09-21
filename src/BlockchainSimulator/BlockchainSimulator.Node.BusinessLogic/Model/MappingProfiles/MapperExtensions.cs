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

        private static void SetupTransactions(MerkleNode merkleNode, HashSet<Transaction.Transaction> transactions)
        {
            switch (merkleNode)
            {
                case Leaf leaf:
                    leaf.Transaction = transactions.FirstOrDefault(t => t.Id == leaf.TransactionId);
                    break;

                case Transaction.Node node:
                    SetupTransactions(node.LeftNode, transactions);
                    SetupTransactions(node.RightNode, transactions);
                    break;
            }
        }
    }
}