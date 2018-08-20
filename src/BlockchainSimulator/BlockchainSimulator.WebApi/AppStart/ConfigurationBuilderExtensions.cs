using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace BlockchainSimulator.WebApi.AppStart
{
    /// <summary>
    /// The configuration builder extensions
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Creates configuration
        /// </summary>
        /// <param name="configurationBuilder">Configuration builder</param>
        /// <param name="jsonFilesPaths">Json files paths</param>
        /// <param name="optionalJsons">Marks jsons as optionals</param>
        /// <param name="args">Arguments for program</param>
        /// <returns>The configuration builder after with additional arguments</returns>
        public static IConfigurationBuilder AddJsonFiles(this IConfigurationBuilder configurationBuilder,
            IEnumerable<string> jsonFilesPaths, bool optionalJsons, params string[] args)
        {
            var inMemoryDictionary = args.Select(arg => arg.Split("|-|")).Where(kv => kv.Length == 2)
                .ToDictionary(kv => kv.First(), kv => kv.Last());

            return jsonFilesPaths.Aggregate(configurationBuilder,
                    (current, path) => current.AddJsonFile(path, optionalJsons, true))
                .AddInMemoryCollection(inMemoryDictionary);
        }
    }
}