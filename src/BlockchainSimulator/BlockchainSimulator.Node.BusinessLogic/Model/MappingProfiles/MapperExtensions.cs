using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.DataAccess.Model;

namespace BlockchainSimulator.Node.BusinessLogic.Model.MappingProfiles
{
    public static class MapperExtensions
    {
        public static BlockBase ManualMap(this IMapper mapper, Blockchain blockchain)
        {
            if (blockchain == null)
            {
                return null;
            }

            var blocks = mapper.Map<List<BlockBase>>(blockchain.Blocks);

            blocks.Where(b => !b.IsGenesis).Cast<Block.Block>().ToList()
                .ForEach(b => b.Parent = blocks.FirstOrDefault(bl => b.ParentId == bl.Id));

            blocks.ForEach(b => SetupTransactions(b.Body.MerkleTree, b.Body.Transactions));

            return blocks.LastOrDefault();
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