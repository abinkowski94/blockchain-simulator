using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Common.Services;
using BlockchainSimulator.Hub.BusinessLogic.Services;
using BlockchainSimulator.Hub.BusinessLogic.Storage;
using BlockchainSimulator.Hub.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BlockchainSimulator.Hub.WebApi.AppStart
{
    /// <summary>
    /// The service registrations
    /// </summary>
    public static class ServicesRegistration
    {
        /// <summary>
        /// Registers all services
        /// </summary>
        /// <param name="services">The services container</param>
        public static void AddHubServices(this IServiceCollection services)
        {
            services.AddTransient<IHttpService, HttpService>();
            services.AddTransient<IFileRepository, FileRepository>();
            services.AddSingleton<IScenarioStorage, ScenarioStorage>();
            services.AddSingleton<ISimulationStorage, SimulationStorage>();
            services.AddSingleton<ISimulationRunnerService, SimulationRunnerService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<QueuedHostedService>();
            services.AddTransient<IScenarioService, ScenarioService>();
            services.AddTransient<ISimulationService, SimulationService>();
        }
    }
}