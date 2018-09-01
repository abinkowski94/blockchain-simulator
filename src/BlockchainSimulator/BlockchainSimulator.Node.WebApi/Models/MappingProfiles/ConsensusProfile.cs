using AutoMapper;
using BlockchainSimulator.Common.Models;
using BlockchainSimulator.Common.Models.Consensus;

namespace BlockchainSimulator.Node.WebApi.Models.MappingProfiles
{
    /// <inheritdoc />
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