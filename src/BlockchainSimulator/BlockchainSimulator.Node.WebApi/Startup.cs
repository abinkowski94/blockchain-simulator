using BlockchainSimulator.Common.AppStart;
using BlockchainSimulator.Node.BusinessLogic.Hubs;
using BlockchainSimulator.Node.WebApi.AppStart;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace BlockchainSimulator.Node.WebApi
{
    /// <summary>
    /// The startup configuration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        /// <summary>
        /// The configuration
        /// </summary>
        private IConfiguration Configuration { get; }

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="configuration">The configuration</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <param name="env">The environment</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDefaultConfiguration(env);
            app.UseSwagger("BlockchainTree simulator (Node API)");
            app.UseSignalR(routes =>
            {
                routes.MapHub<ConsensusHub>("/consensusHub");
                routes.MapHub<SimulationHub>("/simulationHub");
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The service container</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultConfiguration();
            services.AddBlockchainServices(Configuration);
            services.AddSwagger("node");
            services.AddMemoryCache();
            services.AddSignalR();
        }
    }
}