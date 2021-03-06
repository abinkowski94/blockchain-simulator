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
            // Repositories and storage
            services.AddTransient<IFileRepository, FileRepository>();
            services.AddSingleton<IScenarioStorage, ScenarioStorage>();
            services.AddSingleton<ISimulationStorage, SimulationStorage>();

            // Background queue and hosted service
            services.AddSingleton<IBackgroundQueue, BackgroundQueue>();
            services.AddHostedService<QueuedHostedService>();

            // Services
            services.AddTransient<IHttpService, HttpService>();
            services.AddTransient<IScenarioService, ScenarioService>();
            services.AddTransient<IStatisticService, StatisticService>();
            services.AddTransient<ISimulationService, SimulationService>();
            services.AddSingleton<ISimulationRunnerService, SimulationRunnerService>();
        }
    }
}