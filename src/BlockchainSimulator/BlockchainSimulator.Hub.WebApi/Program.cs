using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace BlockchainSimulator.Hub.WebApi
{
    /// <summary>
    /// The program entry class
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point
        /// </summary>
        public static void Main()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("hosting.json", false)
                .AddJsonFile("appsettings.json", false)
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