using AutoMapper;

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
            CreateMap<BusinessLogic.Model.Statistics.Statistic, Statistic>();
        }
    }
}