using AutoMapper;
using BlockchainSimulator.Common.Models;
using BlockchainSimulator.Common.Models.Consensus;
using BlockchainSimulator.Hub.WebApi.Model.Scenarios;

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
            CreateMap<BusinessLogic.Model.Scenarios.BlockchainConfiguration, BlockchainConfiguration>().ReverseMap();
            CreateMap<BusinessLogic.Model.Scenarios.Scenario, Scenario>().ReverseMap();
            CreateMap<BusinessLogic.Model.Consensus.ServerNode, ServerNode>().ReverseMap();
            CreateMap<BusinessLogic.Model.Scenarios.Simulation, Simulation>().ReverseMap();
            CreateMap<BusinessLogic.Model.Scenarios.SimulationSettings, SimulationSettings>().ReverseMap();
        }
    }
}