using AutoMapper;
using BlockchainSimulator.WebApi.Models.MappingProfiles;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainSimulator.WebApi.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected IMapper LocalMapper { get; }

        protected BaseController()
        {
            LocalMapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new TransactionProfile());
                cfg.AddProfile(new ConsensusProfile());
            }));
        }
    }
}