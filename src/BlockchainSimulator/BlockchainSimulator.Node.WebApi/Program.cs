using BlockchainSimulator.Node.WebApi.AppStart;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace BlockchainSimulator.Node.WebApi
{
    /// <summary>
    /// The program entry point
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        /// <summary>
        /// The entry point
        /// </summary>
        /// <param name="args">Arguments for the program</param>
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
#if DEBUG
                .AddJsonFiles(new[]
                {
                    "hosting.json",
                    "appsettings.json"
                }, false, args)
#endif
                .Build();

            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}