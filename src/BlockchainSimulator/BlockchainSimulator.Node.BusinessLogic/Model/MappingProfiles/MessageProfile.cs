using AutoMapper;
using BlockchainSimulator.Node.BusinessLogic.Model.Messages;

namespace BlockchainSimulator.Node.BusinessLogic.Model.MappingProfiles
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<DataAccess.Model.Messages.TransactionMessage, TransactionMessage>().ReverseMap();
            CreateMap<DataAccess.Model.Messages.PrepareMessage, PrepareMessage>().ReverseMap();
            CreateMap<DataAccess.Model.Messages.CommitMessage, CommitMessage>().ReverseMap();
        }
    }
}