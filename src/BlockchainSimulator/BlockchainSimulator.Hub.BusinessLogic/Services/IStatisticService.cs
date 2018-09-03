using BlockchainSimulator.Common.Models.Statistics;
using System.Collections.Generic;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public interface IStatisticService
    {
        void ExtractAndSaveStatistics(List<Statistic> statistics);
    }
}