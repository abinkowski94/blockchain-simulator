using BlockchainSimulator.Common.Models.Statistics;
using System.Collections.Generic;
using System.Linq;
using BlockchainSimulator.Hub.BusinessLogic.Model.Statistics;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public class StatisticService : IStatisticService
    {
        public void ExtractAndSaveStatistics(List<Statistic> statistics)
        {
            CreateBlockchainTree(statistics);
        }

        private static void CreateBlockchainTree(IEnumerable<Statistic> statistics)
        {
            //TODO Fix extraction algorithm
            var blockchainBranches = statistics.SelectMany(s => s.BlockchainStatistics.BlockchainBranches)
                .OrderByDescending(b => b.Count).ToList();

            BlockchainNode root = null;
            if (blockchainBranches.Any())
            {
                root = new BlockchainNode {ChildNodes = new List<BlockchainNode>()};

                foreach (var branch in blockchainBranches)
                {
                    var node = root;

                    foreach (var blockInfo in branch)
                    {
                        if (!node.ChildNodes.Any(i => AreBlockInfoEqual(i.BlockInfo, blockInfo)))
                        {
                            var childNode = new BlockchainNode
                                {BlockInfo = blockInfo, ChildNodes = new List<BlockchainNode>()};
                            node.ChildNodes.Add(childNode);
                            node = childNode;
                        }
                    }
                }
            }

            SaveBlockchainTree(new BlockchainTree {Root = root});
        }

        private static void SaveBlockchainTree(BlockchainTree blockchainTree)
        {
            // TODO
        }

        private static bool AreBlockInfoEqual(BlockInfo blockInfo1, BlockInfo blockInfo2)
        {
            if (blockInfo1 == null || blockInfo2 == null)
            {
                return false;
            }

            return blockInfo1.Id == blockInfo2.Id && blockInfo1.Nonce == blockInfo2.Nonce &&
                   blockInfo1.TimeStamp == blockInfo2.TimeStamp;
        }
    }
}