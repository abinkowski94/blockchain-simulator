using AutoMapper;
using BlockchainSimulator.DataAccess.Model.Block;

namespace BlockchainSimulator.BusinessLogic.Model.Maps
{
    public class BlockchainProfile : Profile
    {
        public BlockchainProfile()
        {
            CreateMap<Body, Block.Body>().ReverseMap();
            CreateMap<Header, Block.Header>().ReverseMap();

            CreateMap<GenesisBlock, Block.GenesisBlock>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Header, opt => opt.MapFrom(src => src.Header))
                .ForMember(dst => dst.Body, opt => opt.MapFrom(src => src.Body))
                .ForMember(dst => dst.BlockJson, opt => opt.MapFrom(src => src.ToString()))
                .ForAllOtherMembers(dst => dst.Ignore());

            CreateMap<DataAccess.Model.Block.Block, Block.Block>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.ParentId, opt => opt.MapFrom(src => src.ParentId))
                .ForMember(dst => dst.Header, opt => opt.MapFrom(src => src.Header))
                .ForMember(dst => dst.Body, opt => opt.MapFrom(src => src.Body))
                .ForMember(dst => dst.BlockJson, opt => opt.MapFrom(src => src.ToString()))
                .ForAllOtherMembers(dst => dst.Ignore());
        }
    }
}