using AutoMapper;
using BlockchainSimulator.Node.WebApi.Models.MappingProfiles;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainSimulator.Node.WebApi.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// The base controller
    /// </summary>
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// The local mapper instance
        /// </summary>
        public IMapper LocalMapper { get; }

        /// <summary>
        /// The controller with configuration
        /// </summary>
        protected BaseController()
        {
            LocalMapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new TransactionProfile());
                cfg.AddProfile(new ConsensusProfile());
                cfg.AddProfile(new ResponsesProfile());
                cfg.AddProfile(new StatisticProfile());
            }));
        }
    }
}