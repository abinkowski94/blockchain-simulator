using System;
using System.IO;
using System.Reflection;
using BlockchainSimulator.Hub.BusinessLogic.Services;
using BlockchainSimulator.Hub.BusinessLogic.Storage;
using BlockchainSimulator.Hub.DataAccess.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

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

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blockchain simulator (Hub API)"); });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The service container</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddTransient<IFileRepository, FileRepository>();
            services.AddSingleton<IScenarioStorage, ScenarioStorage>();
            services.AddSingleton<ISimulationStorage, SimulationStorage>();
            services.AddTransient<IScenarioService, ScenarioService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Blockchain simulator (Hub API)",
                    Version = "v1",
                    Description =
                        "This is the hub of the blockchain simulator where you can preform various simulations",
                    Contact = new Contact
                    {
                        Name = "Augustyn Binkowski",
                        Url = "https://github.com/abinkowski94"
                    },
                    License = new License
                    {
                        Name = "Use under MIT License",
                        Url = "https://github.com/abinkowski94/blockchain-simulator/blob/master/LICENSE"
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }
    }
}