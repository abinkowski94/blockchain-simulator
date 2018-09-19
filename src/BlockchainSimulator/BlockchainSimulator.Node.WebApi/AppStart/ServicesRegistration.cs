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
            services.AddSingleton<IFileRepository, FileRepository>();
            services.AddSingleton<IBlockchainRepository, BlockchainRepository>();

            // Queues and hosted services
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddSingleton<IQueuedHostedServiceSynchronizationContext,
                QueuedHostedServiceSynchronizationContext>();
            services.AddHostedService<QueuedHostedService>();
            services.AddHostedService<ReMiningHostedService>();

            // Services
            services.AddSingleton<IBlockchainService, BlockchainService>();
            services.AddSingleton<ITransactionService, TransactionService>();
            services.AddSingleton<IMiningService, MiningService>();
            services.AddSingleton<IStatisticService, StatisticService>();
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