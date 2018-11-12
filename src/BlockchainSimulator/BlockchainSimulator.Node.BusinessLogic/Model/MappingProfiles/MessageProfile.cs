using AutoMapper;
using BlockchainSimulator.Node.BusinessLogic.Model.Messages;

namespace BlockchainSimulator.Node.BusinessLogic.Model.MappingProfiles
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<DataAccess.Model.Messages.TransactionMessage, TransactionMessage>()
                .Include<DataAccess.Model.Messages.PrepareMessage, PrepareMessage>()
                .Include<DataAccess.Model.Messages.CommitMessage, CommitMessage>();

            CreateMap<TransactionMessage, DataAccess.Model.Messages.TransactionMessage>()
                .Include<PrepareMessage, DataAccess.Model.Messages.PrepareMessage>()
                .Include<CommitMessage, DataAccess.Model.Messages.CommitMessage>();

            CreateMap<DataAccess.Model.Messages.PrepareMessage, PrepareMessage>().ReverseMap();
            CreateMap<DataAccess.Model.Messages.CommitMessage, CommitMessage>().ReverseMap();
        }
    }
}