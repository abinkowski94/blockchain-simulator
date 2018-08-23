using AutoMapper;
using BlockchainSimulator.Hub.WebApi.Model.MappingProfiles;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainSimulator.Hub.WebApi.Controllers
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
                cfg.AddProfile(new ResponsesProfile());
                cfg.AddProfile(new ScenarioAndSimulationProfile());
            }));
        }
    }
}