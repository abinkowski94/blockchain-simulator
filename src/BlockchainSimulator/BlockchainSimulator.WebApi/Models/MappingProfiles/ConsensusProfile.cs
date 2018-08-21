using AutoMapper;
using BlockchainSimulator.Common.Models;

namespace BlockchainSimulator.WebApi.Models.MappingProfiles
{
    /// <summary>
    /// The profile for the consensus
    /// </summary>
    public class ConsensusProfile : Profile
    {
        /// <summary>
        /// The constructor
        /// </summary>
        public ConsensusProfile()
        {
            CreateMap<ServerNode, BusinessLogic.Model.Consensus.ServerNode>()
                .ReverseMap();
        }
    }
}