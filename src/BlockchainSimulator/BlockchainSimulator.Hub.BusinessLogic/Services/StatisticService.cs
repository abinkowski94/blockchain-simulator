using BlockchainSimulator.Common.Models.Statistics;
using BlockchainSimulator.Hub.BusinessLogic.Helpers.Drawing;
using BlockchainSimulator.Hub.BusinessLogic.Model.Scenarios;
using BlockchainSimulator.Hub.BusinessLogic.Model.Statistics;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using OfficeOpenXml;

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

            SaveSettings(directoryPath, settings);
            var treeStats = CreateBlockchainTree(statistics, directoryPath);
            CreateExcelFile(directoryPath, statistics, settings, treeStats);
        }

        private static void CreateExcelFile(string directoryPath, IReadOnlyCollection<Statistic> statistics,
            SimulationSettings settings, Tuple<int, int> treeStats)
        {
            if (statistics.Any())
            {
                var longestBlockchainStatistics =
                    statistics.OrderByDescending(s => s.BlockchainStatistics.TotalTransactionsCount).First();

                using (var package = new ExcelPackage())
                {
                    package.Workbook.Properties.Title = "Simulation results";
                    package.Workbook.Properties.Author = "Augustyn Binkowski";
                    package.Workbook.Properties.Subject = "Blockchain simulation";
                    package.Workbook.Properties.Keywords = "blockchain, simulation, results";

                    CreateCollectiveResultsSheet(settings, treeStats, package, longestBlockchainStatistics);
                    CreateNodesStatisticsSheet(statistics, package);

                    var path = $@"{directoryPath}\simulation-results.xlsx";
                    package.SaveAs(new FileInfo(path));
                }
            }
        }

        private static void CreateNodesStatisticsSheet(IEnumerable<Statistic> statistics, ExcelPackage package)
        {
            var nodesSheet = package.Workbook.Worksheets.Add("Node's statistics");
            var row = 0;

            nodesSheet.Cells[++row, 1].Value = "Mining queue statistics";
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

        private static void CreateCollectiveResultsSheet(SimulationSettings settings, Tuple<int, int> treeStats,
            ExcelPackage package, Statistic longestBlockchainStatistics)
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
            simulationResultsSheet.Cells[++row, 1].Value = "Total queue time for blocks (s)";
            simulationResultsSheet.Cells[row, 2].Value =
                longestBlockchainStatistics.BlockchainStatistics.TotalQueueTimeForBlocks.TotalSeconds;
            simulationResultsSheet.Cells[++row, 1].Value = "Blockchain tree height";
            simulationResultsSheet.Cells[row, 2].Value = treeStats.Item1;
            simulationResultsSheet.Cells[++row, 1].Value = "Blockchain tree nodes count";
            simulationResultsSheet.Cells[row, 2].Value = treeStats.Item2;
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

        private static Tuple<int, int> CreateBlockchainTree(IEnumerable<Statistic> statistics, string directoryPath)
        {
            var statistic = statistics.OrderByDescending(s => s.BlockchainStatistics.TotalTransactionsCount).First();
            var models = statistic.BlockchainStatistics.BlockInfos.Select(i => new NodeModel
            {
                Id = i.UniqueId,
                Name = i.Id,
                ParentId = i.ParentUniqueId ?? "R"
            }).ToList();
            models.Add(new NodeModel {Id = "R", Name = "R", ParentId = string.Empty});
            DrawAndSaveBlockchainTree(models, directoryPath);

            //TODO
            return new Tuple<int, int>(0, models.Count - 1);
        }

        private static void DrawAndSaveBlockchainTree(List<NodeModel> models, string directoryPath)
        {
            var imagePath = $@"{directoryPath}\blockchain-tree.bmp";
            var drawer = new TreeDrawer(imagePath);
            drawer.DrawGraph(models);
        }

        private static void SaveSettings(string directoryPath, SimulationSettings settings)
        {
            var jsonPath = $@"{directoryPath}\simulation-settings.json";
            File.WriteAllText(jsonPath, JsonConvert.SerializeObject(settings));
        }
    }
}