using System;
using BlockchainSimulator.Node.BusinessLogic.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BlockchainSimulator.Node.WebApi.AppStart
{
    /// <summary>
    /// The application lifetime extensions
    /// </summary>
    public static class ApplicationLifetimeExtensions
    {
        /// <summary>
        /// Register on startup actions
        /// </summary>
        /// <param name="app">The application builder</param>
        public static void UseOnStartUp(this IApplicationBuilder app)
        {
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            applicationLifetime.ApplicationStarted.Register(() => OnStartUp(app.ApplicationServices));
        }

        /// <summary>
        /// Cleans the remaining processes
        /// </summary>
        /// <param name="services">The service provider</param>
        private static void OnStartUp(IServiceProvider services)
        {
            var blockchainService = services.GetService<IBlockchainService>();
            blockchainService?.CreateGenesisBlockIfNotExist();
        }
    }
}