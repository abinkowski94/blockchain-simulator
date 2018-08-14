using AutoMapper;

namespace BlockchainSimulator.WebApi.Models.MappingProfiles
{
    public class ConsensusProfile : Profile
    {
        public ConsensusProfile()
        {
            CreateMap<ServerNode, BusinessLogic.Model.Consensus.ServerNode>()
                .ReverseMap();
        }
    }
}