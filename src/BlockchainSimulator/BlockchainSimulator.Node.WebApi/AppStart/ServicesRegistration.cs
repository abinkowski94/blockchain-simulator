using BlockchainSimulator.Common.Services;
using BlockchainSimulator.Node.BusinessLogic.Configurations;
using BlockchainSimulator.Node.BusinessLogic.Providers;
using BlockchainSimulator.Node.BusinessLogic.Providers.Specific;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using BlockchainSimulator.Node.BusinessLogic.Services;
using BlockchainSimulator.Node.BusinessLogic.Services.Specific;
using BlockchainSimulator.Node.BusinessLogic.Validators;
using BlockchainSimulator.Node.BusinessLogic.Validators.Specific;
using BlockchainSimulator.Node.DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlockchainSimulator.Node.WebApi.AppStart
{
    /// <summary>
    /// The service registration extension
    /// </summary>
    public static class ServicesRegistration
    {
        /// <summary>
        ///  Adds the blockchain services to the container
        /// </summary>
        /// <param name="services">The service container</param>
        /// <param name="configuration">The configuration</param>
        /// <returns>The service container</returns>
        public static IServiceCollection AddBlockchainServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Repositories
            services.AddSingleton<IBlockchainRepository, BlockchainRepository>();
            services.AddSingleton<IFileRepository, FileRepository>();

            // Queues and hosted services
            services.AddSingleton<IMiningQueue, MiningQueue>();
            services.AddHostedService<MiningHostedService>();
            services.AddSingleton<Common.Queues.IBackgroundTaskQueue, Common.Queues.BackgroundTaskQueue>();
            services.AddHostedService<Common.Queues.QueuedHostedService>();

            // Services
            services.AddSingleton<ITransactionService, TransactionService>();
            services.AddSingleton<IBlockchainService, BlockchainService>();
            services.AddSingleton<IStatisticService, StatisticService>();
            services.AddSingleton<IMiningService, MiningService>();
            services.AddTransient<IConfigurationService, ConfigurationService>();
            services.AddTransient<IHttpService, HttpService>();

            // Specific
            switch (configuration["Node:Type"])
            {
                case "PoW":
                    // Config
                    services.AddSingleton<IBlockchainConfiguration, ProofOfWorkConfiguration>();

                    // Services
                    services.AddSingleton<IConsensusService, ProofOfWorkConsensusService>();

                    // Providers
                    services.AddTransient<IMerkleTreeProvider, MerkleTreeProvider>();
                    services.AddTransient<IBlockProvider, ProofOfWorkBlockProvider>();

                    // Validators
                    services.AddTransient<IMerkleTreeValidator, MerkleTreeValidator>();
                    services.AddTransient<IBlockchainValidator, ProofOfWorkValidator>();
                    break;
            }

            return services;
        }
    }
}