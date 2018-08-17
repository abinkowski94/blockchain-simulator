using AutoMapper;

namespace BlockchainSimulator.WebApi.Models.MappingProfiles
{
    /// <summary>
    /// The profile for the transactions
    /// </summary>
    public class TransactionProfile : Profile
    {
        /// <summary>
        /// The constructor
        /// </summary>
        public TransactionProfile()
        {
            CreateMap<BusinessLogic.Model.Transaction.Transaction, Transaction>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Sender, opt => opt.MapFrom(src => src.Sender))
                .ForMember(dst => dst.Recipient, opt => opt.MapFrom(src => src.Recipient))
                .ForMember(dst => dst.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dst => dst.Fee, opt => opt.MapFrom(src => src.Fee))
                .ForMember(dst => dst.RegistrationTime, opt => opt.MapFrom(src => src.RegistrationTime))
                .ForMember(dst => dst.TransactionDetails, opt => opt.MapFrom(src => src.TransactionDetails))
                .ReverseMap()
                .ForAllOtherMembers(dst => dst.Ignore());

            CreateMap<BusinessLogic.Model.Transaction.TransactionDetails, TransactionDetails>()
                .ReverseMap();
        }
    }
}