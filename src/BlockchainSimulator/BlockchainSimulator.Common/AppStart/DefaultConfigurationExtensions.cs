using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BlockchainSimulator.Common.AppStart
{
    /// <summary>
    /// The default configuration extensions
    /// </summary>
    public static class DefaultConfigurationExtensions
    {
        /// <summary>
        /// Uses default configuration
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <param name="env">The environment</param>
        public static void UseDefaultConfiguration(this IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        /// <summary>
        /// Adds default configuration
        /// </summary>
        /// <param name="services">The services container</param>
        public static void AddDefaultConfiguration(this IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }
    }
}