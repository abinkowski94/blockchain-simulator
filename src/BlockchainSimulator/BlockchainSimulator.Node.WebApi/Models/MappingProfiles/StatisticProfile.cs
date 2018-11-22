using System.Linq;
using AutoMapper;
using BlockchainSimulator.Common.Models.Statistics;

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
            CreateMap<BlockchainSimulator.Node.BusinessLogic.Model.Statistics.BlockchainStatistics, BlockchainStatistics
            >();
            CreateMap<BlockchainSimulator.Node.BusinessLogic.Model.Statistics.MiningQueueStatistics,
                MiningQueueStatistics>();
            CreateMap<BlockchainSimulator.Node.BusinessLogic.Model.Statistics.BlockInfo, BlockInfo>();

            CreateMap<BlockchainSimulator.Node.BusinessLogic.Model.Staking.Epoch, Epoch>()
                .ForMember(dst => dst.Number, opt => opt.MapFrom(src => src.Number))
                .ForMember(dst => dst.HasFinalized, opt => opt.MapFrom(src => src.HasFinalized))
                .ForMember(dst => dst.HasPrepared, opt => opt.MapFrom(src => src.HasPrepared))
                .ForMember(dst => dst.TotalStake, opt => opt.MapFrom(src => src.TotalStake))
                .ForMember(dst => dst.FinalizedBlockId, opt => opt.MapFrom(src => src.FinalizedBlockId))
                .ForMember(dst => dst.PreparedBlockId, opt => opt.MapFrom(src => src.PreparedBlockId))
                .ForMember(dst => dst.CheckpointsWithCommitStakes,
                    opt => opt.MapFrom(
                        src => src.CheckpointsWithCommitStakes.ToDictionary(kv => kv.Key, kv => kv.Value)))
                .ForMember(dst => dst.CheckpointsWithPrepareStakes,
                    opt => opt.MapFrom(
                        src => src.CheckpointsWithPrepareStakes.ToDictionary(kv => kv.Key, kv => kv.Value)))
                .ForAllOtherMembers(dst => dst.Ignore());
        }
    }
}