using System.Collections.Generic;
using System.Linq;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Services;

namespace BlockchainSimulator.Node.BusinessLogic.Providers
{
    public class MerkleTreeProvider : IMerkleTreeProvider
    {
        public Model.Transaction.Node GetMerkleTree(HashSet<Transaction> transactions)
        {
            if (transactions == null)
            {
                return null;
            }

            var leafs = transactions.OrderByDescending(t => t.Fee)
                .Select(t => new Leaf
                    {
                        TransactionId = t.Id,
                        Transaction = t,
                        Hash = EncryptionService.GetSha256Hash(t.TransactionJson)
                    }
                ).Cast<MerkleNode>().ToList();

            return CombineNodes(leafs).FirstOrDefault() as Model.Transaction.Node;
        }

        private static List<MerkleNode> CombineNodes(List<MerkleNode> nodes)
        {
            if (nodes.Count <= 1)
            {
                var node = nodes.FirstOrDefault();
                if (node is Leaf)
                {
                    return new List<MerkleNode>
                    {
                        new Model.Transaction.Node
                        {
                            LeftNode = node,
                            RightNode = null,
                            Hash = EncryptionService.GetSha256Hash($"{node.Hash}{null}")
                        }
                    };
                }

                return nodes;
            }

            var resultNodes = new List<MerkleNode>();

            for (var i = 0; i < nodes.Count; i = i + 2)
            {
                var rightNode = i + 1 < nodes.Count ? nodes[i + 1] : null;

                var result = new Model.Transaction.Node
                {
                    LeftNode = nodes[i],
                    RightNode = rightNode,
                    Hash = EncryptionService.GetSha256Hash($"{nodes[i].Hash}{rightNode?.Hash}")
                };

                resultNodes.Add(result);
            }

            return CombineNodes(resultNodes);
        }
    }
}