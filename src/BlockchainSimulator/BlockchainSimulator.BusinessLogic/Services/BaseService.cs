using AutoMapper;
using BlockchainSimulator.BusinessLogic.Model.Maps;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public abstract class BaseService
    {
        public IMapper LocalMapper { get; }

        public BaseService()
        {
            LocalMapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new TransactionProfile());
                cfg.AddProfile(new BlockchainProfile());
            }));
        }
    }
}