using AutoMapper;
using BlockchainSimulator.Common.Models;

namespace BlockchainSimulator.Hub.WebApi.Model.MappingProfiles
{
    /// <inheritdoc />
    /// <summary>
    /// The scenario profile
    /// </summary>
    public class ScenarioAndSimulationProfile : Profile
    {
        /// <summary>
        /// The constructor
        /// </summary>
        public ScenarioAndSimulationProfile()
        {
            CreateMap<BusinessLogic.Model.BlockchainConfiguration, BlockchainConfiguration>().ReverseMap();
            CreateMap<BusinessLogic.Model.Scenario, Scenario>().ReverseMap();
            CreateMap<BusinessLogic.Model.ServerNode, ServerNode>().ReverseMap();
            CreateMap<BusinessLogic.Model.Simulation, Simulation>().ReverseMap();
            CreateMap<BusinessLogic.Model.SimulationSettings, SimulationSettings>().ReverseMap();
        }
    }
}