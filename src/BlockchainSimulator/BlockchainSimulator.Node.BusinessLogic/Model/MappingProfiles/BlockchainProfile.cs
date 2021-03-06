using AutoMapper;
using BlockchainSimulator.Node.DataAccess.Model.Block;

namespace BlockchainSimulator.Node.BusinessLogic.Model.MappingProfiles
{
    public class BlockchainProfile : Profile
    {
        public BlockchainProfile()
        {
            CreateMap<Body, Block.Body>().ReverseMap();
            CreateMap<Header, Block.Header>().ReverseMap();

            CreateMap<BlockBase, Block.BlockBase>()
                .Include<GenesisBlock, Block.GenesisBlock>()
                .Include<DataAccess.Model.Block.Block, Block.Block>();

            CreateMap<Block.BlockBase, BlockBase>()
                .Include<Block.GenesisBlock, GenesisBlock>()
                .Include<Block.Block, DataAccess.Model.Block.Block>();

            CreateMap<GenesisBlock, Block.GenesisBlock>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.UniqueId, opt => opt.MapFrom(src => src.UniqueId))
                .ForMember(dst => dst.Header, opt => opt.MapFrom(src => src.Header))
                .ForMember(dst => dst.Body, opt => opt.MapFrom(src => src.Body))
                .ForMember(dst => dst.QueueTime, opt => opt.MapFrom(src => src.QueueTime))
                .ForMember(dst => dst.Depth, opt => opt.MapFrom(src => src.Depth))
                .ReverseMap()
                .ForAllOtherMembers(dst => dst.Ignore());

            CreateMap<DataAccess.Model.Block.Block, Block.Block>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.UniqueId, opt => opt.MapFrom(src => src.UniqueId))
                .ForMember(dst => dst.ParentUniqueId, opt => opt.MapFrom(src => src.ParentUniqueId))
                .ForMember(dst => dst.Header, opt => opt.MapFrom(src => src.Header))
                .ForMember(dst => dst.Body, opt => opt.MapFrom(src => src.Body))
                .ForMember(dst => dst.QueueTime, opt => opt.MapFrom(src => src.QueueTime))
                .ForMember(dst => dst.Depth, opt => opt.MapFrom(src => src.Depth))
                .ReverseMap()
                .ForAllOtherMembers(dst => dst.Ignore());
        }
    }
}