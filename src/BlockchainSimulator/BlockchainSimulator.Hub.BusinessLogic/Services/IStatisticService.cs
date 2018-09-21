using BlockchainSimulator.Common.Models.Statistics;
using BlockchainSimulator.Hub.BusinessLogic.Model.Scenarios;
using System.Collections.Generic;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public interface IStatisticService
    {
        void ExtractAndSaveStatistics(List<Statistic> statistics, SimulationSettings settings, string scenarioId);
    }
}