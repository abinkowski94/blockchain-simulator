using BlockchainSimulator.Common.AppStart;
using BlockchainSimulator.Hub.WebApi.AppStart;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlockchainSimulator.Hub.WebApi
{
    /// <summary>
    /// The startup class
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// The configuration
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
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
            app.UseSwagger("Blockchain simulator (Hub API)");
            app.UseOnShutdownCleanup();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The service container</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultConfiguration();
            services.AddHubServices();
            services.AddSwagger("hub");
        }
    }
}