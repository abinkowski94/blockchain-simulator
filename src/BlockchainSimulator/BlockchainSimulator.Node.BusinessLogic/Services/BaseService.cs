using AutoMapper;
using BlockchainSimulator.Node.BusinessLogic.Model.MappingProfiles;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public abstract class BaseService
    {
        protected IMapper LocalMapper { get; }

        protected BaseService()
        {
            LocalMapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new TransactionProfile());
                cfg.AddProfile(new BlockchainProfile());
            }));
        }
    }
}