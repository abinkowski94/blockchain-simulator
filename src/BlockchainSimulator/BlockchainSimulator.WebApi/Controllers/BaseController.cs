using AutoMapper;
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
                
            }));
        }
    }
}