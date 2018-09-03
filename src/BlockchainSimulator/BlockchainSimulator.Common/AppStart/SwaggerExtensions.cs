using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;

namespace BlockchainSimulator.Common.AppStart
{
    /// <summary>
    /// The swagger extensions
    /// </summary>
    public static class SwaggerExtensions
    {
        /// <summary>
        /// Adds swagger
        /// </summary>
        /// <param name="services">The services collection</param>
        /// <param name="serviceName">The service name (hub or node)</param>
        public static void AddSwagger(this IServiceCollection services, string serviceName)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = $"Blockchain simulator ({serviceName} API)",
                    Version = "v1",
                    Description = $"This is the {serviceName} of the blockchain simulator",
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

        /// <summary>
        /// Use swagger
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="name">Name of the swagger application</param>
        public static void UseSwagger(this IApplicationBuilder app, string name)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", name); });
        }
    }
}