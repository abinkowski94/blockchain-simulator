using AutoMapper;
using BlockchainSimulator.Common.Models.Statistics;
using BS = BlockchainSimulator.Node.BusinessLogic.Model.Statistics;

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
            CreateMap<BS.Statistic, Statistic>();
            CreateMap<BS.BlockchainStatistics, BlockchainStatistics>();
            CreateMap<BS.MiningQueueStatistics, MiningQueueStatistics>();
            CreateMap<BS.BlockInfo, BlockInfo>();
        }
    }
}