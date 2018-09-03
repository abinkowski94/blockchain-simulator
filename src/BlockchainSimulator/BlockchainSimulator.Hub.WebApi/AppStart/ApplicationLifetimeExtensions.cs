using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Hub.BusinessLogic.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlockchainSimulator.Hub.WebApi.AppStart
{
    /// <summary>
    /// The application lifetime extensions
    /// </summary>
    public static class ApplicationLifetimeExtensions
    {
        /// <summary>
        /// Register on shutdown action
        /// </summary>
        /// <param name="app">The application builder</param>
        public static void UseOnShutdownCleanup(this IApplicationBuilder app)
        {
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            applicationLifetime.ApplicationStopping.Register(() => OnShutdown(app.ApplicationServices));
        }

        /// <summary>
        /// Cleans the remaining processes
        /// </summary>
        /// <param name="services">The service provider</param>
        private static void OnShutdown(IServiceProvider services)
        {
            services.GetService<QueuedHostedService>().Dispose();
            services.GetService<IScenarioStorage>().Dispose();

            Console.WriteLine("Cleanup complete!");
        }
    }
}