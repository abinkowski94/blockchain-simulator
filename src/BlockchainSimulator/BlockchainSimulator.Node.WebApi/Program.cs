using BlockchainSimulator.Node.WebApi.AppStart;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore;

namespace BlockchainSimulator.Node.WebApi
{
    /// <summary>
    /// The program entry point
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        /// <summary>
        /// The web-host builder
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSetting("detailedErrors", "true")
                .Build();

        /// <summary>
        /// The entry point
        /// </summary>
        /// <param name="args">Arguments for the program</param>
        public static void Main(string[] args)
        {
#if DEBUG
            var config = new ConfigurationBuilder()
                .AddJsonFiles(new[] {"hosting.json", "appsettings.json"}, false, args)
                .Build();

            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Run();
#else
            BuildWebHost(args).Run();
#endif
        }
    }
}