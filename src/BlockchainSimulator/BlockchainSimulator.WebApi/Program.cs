using BlockchainSimulator.WebApi.AppStart;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace BlockchainSimulator.WebApi
{
    /// <summary>
    /// The program entry point
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The entry point
        /// </summary>
        /// <param name="args">Arguments for the program</param>
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFiles(new[] {"hosting.json", "appsettings.json"}, args)
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