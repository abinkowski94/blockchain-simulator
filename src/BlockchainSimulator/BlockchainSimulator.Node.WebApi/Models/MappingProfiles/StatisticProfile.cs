using AutoMapper;
using BlockchainSimulator.Common.Models;

namespace BlockchainSimulator.Node.WebApi.Models.MappingProfiles
{
    /// <inheritdoc />
    /// <summary>
    /// The statistic mapping profile
    /// </summary>
    public class StatisticProfile : Profile
    {
        /// <summary>
        /// The constructor
        /// </summary>
        public StatisticProfile()
        {
            CreateMap<BlockchainSimulator.Node.BusinessLogic.Model.Statistics.Statistic, Statistic>();
            CreateMap<BlockchainSimulator.Node.BusinessLogic.Model.Statistics.BlockchainStatistics, BlockchainStatistics>();
            CreateMap<BlockchainSimulator.Node.BusinessLogic.Model.Statistics.MiningQueueStatistics, MiningQueueStatistics>();
        }
    }
}