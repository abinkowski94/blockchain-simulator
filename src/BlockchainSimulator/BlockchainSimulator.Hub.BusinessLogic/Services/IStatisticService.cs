using BlockchainSimulator.Common.Models.Statistics;
using System.Collections.Generic;
using BlockchainSimulator.Hub.BusinessLogic.Model.Scenarios;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public interface IStatisticService
    {
        void ExtractAndSaveStatistics(List<Statistic> statistics, SimulationSettings settings, string scenarioId);
    }
}