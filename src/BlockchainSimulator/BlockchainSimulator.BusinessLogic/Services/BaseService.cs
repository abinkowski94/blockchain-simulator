using AutoMapper;
using BlockchainSimulator.BusinessLogic.Model.MappingProfiles;

namespace BlockchainSimulator.BusinessLogic.Services
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