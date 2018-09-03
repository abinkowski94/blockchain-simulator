using System.Collections.Generic;
using BlockchainSimulator.Common.Models.Statistics;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public interface IStatisticService
    {
        void ExtractAndSaveStatistics(List<Statistic> statistics);
    }
}