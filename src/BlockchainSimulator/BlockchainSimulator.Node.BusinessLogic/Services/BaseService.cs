using AutoMapper;
using BlockchainSimulator.Common.Models;
using BlockchainSimulator.Node.BusinessLogic.Model.MappingProfiles;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public abstract class BaseService
    {
        private readonly IConfigurationService _configurationService;

        protected IMapper LocalMapper { get; }
        protected BlockchainNodeConfiguration BlockchainNodeConfiguration => _configurationService.GetConfiguration();

        protected BaseService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;

            LocalMapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new TransactionProfile());
                cfg.AddProfile(new BlockchainProfile());
                cfg.AddProfile(new MessageProfile());
            }));
        }
    }
}