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
                .AddJsonFile("hosting.json", optional: false)
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