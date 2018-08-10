using AutoMapper;
using BlockchainSimulator.DataAccess.Model.Transaction;

namespace BlockchainSimulator.BusinessLogic.Model.MappingProfiles
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<DataAccess.Model.Transaction.Transaction, Transaction.Transaction>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Sender, opt => opt.MapFrom(src => src.Sender))
                .ForMember(dst => dst.Recipient, opt => opt.MapFrom(src => src.Recipient))
                .ForMember(dst => dst.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dst => dst.Fee, opt => opt.MapFrom(src => src.Fee))
                .ForMember(dst => dst.TransactionJson, opt => opt.MapFrom(src => src.ToString()))
                .ForAllOtherMembers(dst => dst.Ignore());

            CreateMap<Transaction.Transaction, DataAccess.Model.Transaction.Transaction>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Sender, opt => opt.MapFrom(src => src.Sender))
                .ForMember(dst => dst.Recipient, opt => opt.MapFrom(src => src.Recipient))
                .ForMember(dst => dst.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dst => dst.Fee, opt => opt.MapFrom(src => src.Fee))
                .ForAllOtherMembers(dst => dst.Ignore());

            CreateMap<MerkleNode, Transaction.MerkleNode>()
                .Include<Node, Transaction.Node>()
                .Include<Leaf, Transaction.Leaf>();
            
            CreateMap<Transaction.MerkleNode, MerkleNode>()
                .Include<Transaction.Node, Node>()
                .Include<Transaction.Leaf, Leaf>();
            
            CreateMap<Node, Transaction.Node>().ReverseMap();

            CreateMap<Leaf, Transaction.Leaf>()
                .ForMember(dst => dst.Transaction, opt => opt.Ignore())
                .ForMember(dst => dst.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dst => dst.Hash, opt => opt.MapFrom(src => src.Hash))
                .ForAllOtherMembers(dst => dst.Ignore());

            CreateMap<Transaction.Leaf, Leaf>()
                .ForMember(dst => dst.TransactionId, opt => opt.ResolveUsing(src => src.Transaction?.Id))
                .ForMember(dst => dst.Hash, opt => opt.MapFrom(src => src.Hash))
                .ForAllOtherMembers(dst => dst.Ignore());
        }
    }
}