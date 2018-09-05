using System;
using BlockchainSimulator.Common.Models.Statistics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Hub.BusinessLogic.Helpers.Drawing;
using BlockchainSimulator.Hub.BusinessLogic.Model.Scenarios;
using BlockchainSimulator.Hub.BusinessLogic.Model.Statistics;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly string _directory;

        public StatisticService(IHostingEnvironment environment)
        {
            _directory = environment.ContentRootPath ?? Directory.GetCurrentDirectory();
        }

        public void ExtractAndSaveStatistics(List<Statistic> statistics, SimulationSettings settings, string scenarioId)
        {
            var directoryPath = $@"{_directory}\statistics\{scenarioId}\{DateTime.UtcNow:yyyy-MM-dd-hh-mm-ss}";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            
            CreateBlockchainTree(statistics, directoryPath);
        }

        private static void CreateBlockchainTree(IEnumerable<Statistic> statistics, string directoryPath)
        {
            var blockchainBranches = statistics.SelectMany(s => s.BlockchainStatistics.BlockchainBranches)
                .OrderByDescending(b => b.Count).ToList();

            var tempRes = new List<BlockInfo>();
            blockchainBranches.SelectMany(b => b).ForEach(b =>
            {
                if (!tempRes.Any(t => AreBlockInfoEqual(t, b)))
                {
                    tempRes.Add(b);
                }
            });


            var root = new BlockchainNode {ChildNodes = new List<BlockchainNode>()};

            foreach (var branch in blockchainBranches)
            {
                var node = root;
                foreach (var blockInfo in branch)
                {
                    if (node == null)
                    {
                        continue;
                    }

                    if (!node.ChildNodes.Any(n => AreBlockInfoEqual(n.BlockInfo, blockInfo)))
                    {
                        var newNode = new BlockchainNode
                        {
                            BlockInfo = blockInfo,
                            ChildNodes = new List<BlockchainNode>()
                        };
                        node.ChildNodes.Add(newNode);
                        node = newNode;
                    }
                    else
                    {
                        node = node.ChildNodes.FirstOrDefault(n => AreBlockInfoEqual(n.BlockInfo, blockInfo));
                    }
                }
            }

            SaveBlockchainTree(new BlockchainTree {Root = root}, directoryPath);
        }

        private static void SaveBlockchainTree(BlockchainTree blockchainTree, string directoryPath)
        {
            var jsonPath = $@"{directoryPath}\blockchain-tree.json";
            File.WriteAllText(jsonPath, JsonConvert.SerializeObject(blockchainTree));

            var imagePath = $@"{directoryPath}\blockchain-tree.bmp";
            var drawer = new TreeDrawer(imagePath);

            var data = GetDataForDrawer(blockchainTree.Root);
            drawer.DrawGraph(data);
        }

        private static List<NodeModel> GetDataForDrawer(BlockchainNode node, Guid? parentId = null,
            List<NodeModel> result = null)
        {
            if (result == null)
            {
                result = new List<NodeModel>();
            }

            if (node != null)
            {
                var nodeId = Guid.NewGuid();
                result.Add(new NodeModel
                {
                    Id = nodeId.ToString() ?? "R",
                    Name = node.BlockInfo?.Id ?? "R",
                    ParentId = parentId?.ToString() ?? string.Empty
                });

                node.ChildNodes?.ForEach(n => GetDataForDrawer(n, nodeId, result));
            }

            return result;
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