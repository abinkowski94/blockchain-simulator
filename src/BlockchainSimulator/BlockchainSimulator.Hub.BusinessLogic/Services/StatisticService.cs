using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Common.Models.Statistics;
using BlockchainSimulator.Hub.BusinessLogic.Helpers.Drawing;
using BlockchainSimulator.Hub.BusinessLogic.Model.Scenarios;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

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

            try
            {
                SaveSettings(directoryPath, settings);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            try
            {
                CreateExcelFile(directoryPath, statistics, settings);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            try
            {
                CreateBlockchainTrees(statistics, directoryPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void SaveSettings(string directoryPath, SimulationSettings settings)
        {
            var jsonPath = $@"{directoryPath}\simulation-settings.json";
            File.WriteAllText(jsonPath, JsonConvert.SerializeObject(settings));
        }

        private static void CreateExcelFile(string directoryPath, IReadOnlyCollection<Statistic> statistics,
            SimulationSettings settings)
        {
            if (statistics.Any())
            {
                var longestBlockchainStatistics =
                    statistics.OrderByDescending(s => s.BlockchainStatistics.TotalTransactionsCount).First();
                
                longestBlockchainStatistics.BlockchainStatistics.BlockInfos = statistics
                    .SelectMany(s => s.BlockchainStatistics.BlockInfos).GroupBy(bi => bi.UniqueId)
                    .Select(g => g.First()).ToList();

                using (var package = new ExcelPackage())
                {
                    package.Workbook.Properties.Title = "Simulation results";
                    package.Workbook.Properties.Author = "Augustyn Binkowski";
                    package.Workbook.Properties.Subject = "Blockchain simulation";
                    package.Workbook.Properties.Keywords = "blockchain, simulation, results";

                    CreateCollectiveResultsSheet(settings, package, longestBlockchainStatistics);
                    CreateNodesStatisticsSheet(statistics.OrderBy(s => s.NodeId), package);

                    var path = $@"{directoryPath}\simulation-results.xlsx";
                    package.SaveAs(new FileInfo(path));
                }
            }
        }

        private static void CreateCollectiveResultsSheet(SimulationSettings settings, ExcelPackage package,
            Statistic longestBlockchainStatistics)
        {
            var simulationResultsSheet = package.Workbook.Worksheets.Add("Simulation results collectively");
            var row = 0;

            simulationResultsSheet.Cells[++row, 1].Value = "The simulation results";
            simulationResultsSheet.Cells[row, 1].Style.Font.Bold = true;
            simulationResultsSheet.Cells[row, 1].Style.Font.Size = 13;
            row += 2;

            simulationResultsSheet.Cells[++row, 1].Value = "Simulation settings";
            simulationResultsSheet.Cells[row, 1].Style.Font.Bold = true;

            simulationResultsSheet.Cells[++row, 1].Value = "Consensus type";
            simulationResultsSheet.Cells[row, 2].Value = longestBlockchainStatistics.NodeType;
            simulationResultsSheet.Cells[++row, 1].Value = "Version";
            simulationResultsSheet.Cells[row, 2].Value = longestBlockchainStatistics.Version;
            simulationResultsSheet.Cells[++row, 1].Value = "Block size";
            simulationResultsSheet.Cells[row, 2].Value = longestBlockchainStatistics.BlockSize;
            simulationResultsSheet.Cells[++row, 1].Value = "Target";
            simulationResultsSheet.Cells[row, 2].Value = longestBlockchainStatistics.Target;
            simulationResultsSheet.Cells[++row, 1].Value = "Transactions sent";
            simulationResultsSheet.Cells[row, 2].Value = settings.TransactionsSent;
            row += 2;

            simulationResultsSheet.Cells[++row, 1].Value = "Blockchain statistics";
            simulationResultsSheet.Cells[row, 1].Style.Font.Bold = true;

            simulationResultsSheet.Cells[++row, 1].Value = "Blocks count";
            simulationResultsSheet.Cells[row, 2].Value =
                longestBlockchainStatistics.BlockchainStatistics.BlocksCount;
            simulationResultsSheet.Cells[++row, 1].Value = "Transactions count";
            simulationResultsSheet.Cells[row, 2].Value =
                longestBlockchainStatistics.BlockchainStatistics.TotalTransactionsCount;
            simulationResultsSheet.Cells[++row, 1].Value = "Total queue time (s)";
            simulationResultsSheet.Cells[row, 2].Value =
                longestBlockchainStatistics.BlockchainStatistics.TotalQueueTimeForBlocks.TotalSeconds;
            simulationResultsSheet.Cells[++row, 1].Value = "Blockchain tree height";
            simulationResultsSheet.Cells[row, 2].Value =
                longestBlockchainStatistics.BlockchainStatistics.BlockInfos.Select(i => Convert.ToInt32(i.Id, 16))
                    .Max() + 1;
            simulationResultsSheet.Cells[++row, 1].Value = "Blockchain tree nodes count";
            simulationResultsSheet.Cells[row, 2].Value =
                longestBlockchainStatistics.BlockchainStatistics.BlockInfos.Count;
            row += 2;

            simulationResultsSheet.Cells[++row, 1].Value = "Transactions statistics";
            simulationResultsSheet.Cells[row, 1].Style.Font.Bold = true;

            simulationResultsSheet.Cells[++row, 1].Value = "Block id";
            simulationResultsSheet.Cells[row, 2].Value = "Block queue time (s)";
            simulationResultsSheet.Cells[row, 3].Value = "Transaction id";
            simulationResultsSheet.Cells[row, 4].Value = "Transaction registration time";
            simulationResultsSheet.Cells[row, 5].Value = "Transaction confirmation time (s)";
            simulationResultsSheet.Cells[row, 6].Value = "Transaction fee";

            foreach (var stat in longestBlockchainStatistics.BlockchainStatistics.TransactionsStatistics)
            {
                simulationResultsSheet.Cells[++row, 1].Value = stat.BlockId;
                simulationResultsSheet.Cells[row, 2].Value = stat.BlockQueueTime.TotalSeconds;
                simulationResultsSheet.Cells[row, 3].Value = stat.TransactionId;
                simulationResultsSheet.Cells[row, 4].Value =
                    stat.TransactionRegistrationTime.ToString(CultureInfo.InvariantCulture);
                simulationResultsSheet.Cells[row, 5].Value = stat.TransactionConfirmationTime.TotalSeconds;
                simulationResultsSheet.Cells[row, 6].Value = stat.TransactionFee;
            }

            simulationResultsSheet.Cells[1, 1, row, 6].AutoFitColumns();
        }

        private static void CreateNodesStatisticsSheet(IEnumerable<Statistic> statistics, ExcelPackage package)
        {
            var nodesSheet = package.Workbook.Worksheets.Add("Nodes statistics");
            var row = 0;

            nodesSheet.Cells[++row, 1].Value = "Queues statistics";
            nodesSheet.Cells[row, 1].Style.Font.Bold = true;
            nodesSheet.Cells[row, 1].Style.Font.Size = 13;
            row += 2;

            nodesSheet.Cells[++row, 1].Value = "Node Id";
            nodesSheet.Cells[row, 2].Value = "Total mining attempts";
            nodesSheet.Cells[row, 3].Value = "Max queue length";
            nodesSheet.Cells[row, 4].Value = "Average queue time (s)";
            nodesSheet.Cells[row, 5].Value = "Total queue time (s)";
            nodesSheet.Cells[row, 6].Value = "Total abandoned blocks count";
            nodesSheet.Cells[row, 7].Value = "Total rejected incoming blocks count";

            foreach (var stat in statistics)
            {
                nodesSheet.Cells[++row, 1].Value = stat.NodeId;
                nodesSheet.Cells[row, 2].Value = stat.MiningQueueStatistics.TotalMiningAttemptsCount;
                nodesSheet.Cells[row, 3].Value = stat.MiningQueueStatistics.MaxQueueLength;
                nodesSheet.Cells[row, 4].Value = stat.MiningQueueStatistics.AverageQueueTime.TotalSeconds;
                nodesSheet.Cells[row, 5].Value = stat.MiningQueueStatistics.TotalQueueTime.TotalSeconds;
                nodesSheet.Cells[row, 6].Value = stat.MiningQueueStatistics.AbandonedBlocksCount;
                nodesSheet.Cells[row, 7].Value = stat.MiningQueueStatistics.RejectedIncomingBlockchainCount;
            }

            nodesSheet.Cells[1, 1, row, 7].AutoFitColumns();
        }

        private static void CreateBlockchainTrees(List<Statistic> statistics, string directoryPath)
        {
            var treeInfos = statistics.SelectMany(s => s.BlockchainStatistics.BlockInfos).GroupBy(i => i.UniqueId)
                .Select(g => g.First()).ToList();

            var treeNodes = GetTreeNodes(treeInfos);
            DrawAndSaveBlockchainTree(treeNodes, directoryPath, "blockchain-tree.jpg");

            var compressedTreeNodes = GetTreeNodes(treeInfos, true);
            DrawAndSaveBlockchainTree(compressedTreeNodes, directoryPath, "blockchain-tree-compressed.jpg");

            var treesPaths = $"{directoryPath}/trees";
            if (!Directory.Exists(treesPaths))
            {
                Directory.CreateDirectory(treesPaths);
            }

            statistics.ForEach(s =>
            {
                treeInfos = s.BlockchainStatistics.BlockInfos.GroupBy(i => i.UniqueId).Select(g => g.First()).ToList();
                treeNodes = GetTreeNodes(treeInfos);
                DrawAndSaveBlockchainTree(treeNodes, treesPaths, $"{s.NodeId}-blockchain-tree.jpg");
            });
        }

        private static List<NodeModel> GetTreeNodes(IEnumerable<BlockInfo> mainTreeInfos, bool compressed = false)
        {
            var root = new NodeModel { Id = "R", Content = "R", ParentId = string.Empty };
            var treeNodes = mainTreeInfos.Select(i => new NodeModel
            {
                Id = i.UniqueId,
                Content = Convert.ToInt64(i.Id, 16).ToString(),
                ParentId = i.ParentUniqueId ?? "R"
            }).ToList();

            treeNodes.Add(root);
            treeNodes.ForEach(n =>
            {
                n.Parent = treeNodes.FirstOrDefault(node => node.Id == n.ParentId);
                n.Children = treeNodes.Where(node => node.ParentId == n.Id).ToList();
            });

            ColorLongestNodesPath(treeNodes);
            if (compressed)
            {
                CompressChains(root, treeNodes);
            }

            return SortTreeNodes(root);
        }

        private static void ColorLongestNodesPath(IEnumerable<NodeModel> treeNodes)
        {
            var deepestNode = treeNodes.OrderByDescending(n => n.Depth).FirstOrDefault();
            while (deepestNode != null)
            {
                deepestNode.Colour = Pens.Green;
                deepestNode = deepestNode.Parent;
            }
        }

        private static void CompressChains(NodeModel root, List<NodeModel> treeNodes)
        {
            // Set fixed height and compress the chains
            treeNodes.ForEach(n => n.Height = n.Height);
            CompressChainsOfNodes(root, treeNodes);
            treeNodes.ForEach(n =>
            {
                n.Parent = treeNodes.FirstOrDefault(node => node.Id == n.ParentId);
                n.Children = treeNodes.Where(node => node.ParentId == n.Id).ToList();
            });
        }

        private static NodeModel CompressChainsOfNodes(NodeModel root, List<NodeModel> treeNodes)
        {
            if (root.Children.Count > 1)
            {
                root.Children.ForEach(c => CompressChainsOfNodes(c, treeNodes));
            }
            else if (root.Children.Count == 1)
            {
                var child = root.Children.First();
                var tailNode = CompressChainsOfNodes(child, treeNodes);

                if (child == tailNode)
                {
                    return child;
                }

                child.IsCompressed = true;
                child.Content = "...";
                child.Children.ForEach(c => treeNodes.Remove(c));
                tailNode.ParentId = child.Id;
                if (!treeNodes.Contains(tailNode))
                {
                    treeNodes.Add(tailNode);
                }

                return tailNode;
            }

            return root;
        }

        private static List<NodeModel> SortTreeNodes(NodeModel root, List<NodeModel> result = null)
        {
            if (result == null)
            {
                result = new List<NodeModel> { root };
            }
            else
            {
                result.Add(root);
            }

            if (root.Children.Any())
            {
                root.Children.OrderByDescending(c => c.Height).ThenByDescending(c => c.IsCompressed)
                    .ForEach(c => SortTreeNodes(c, result));
            }

            return result;
        }

        private static void DrawAndSaveBlockchainTree(List<NodeModel> models, string directoryPath, string fileName)
        {
            var imagePath = $@"{directoryPath}\{fileName}";
            var drawer = new TreeDrawer(imagePath);
            drawer.DrawGraph(models);
        }
    }
}