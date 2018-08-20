using BlockchainSimulator.BusinessLogic.Configurations;
using BlockchainSimulator.BusinessLogic.Providers;
using BlockchainSimulator.BusinessLogic.Providers.Specific;
using BlockchainSimulator.BusinessLogic.Queues.BackgroundTasks;
using BlockchainSimulator.BusinessLogic.Queues.MiningQueue;
using BlockchainSimulator.BusinessLogic.Services;
using BlockchainSimulator.BusinessLogic.Services.Specific;
using BlockchainSimulator.BusinessLogic.Validators;
using BlockchainSimulator.BusinessLogic.Validators.Specific;
using BlockchainSimulator.DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlockchainSimulator.WebApi.AppStart
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

            // Queues
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<MiningHostedService>();
            services.AddSingleton<IMiningQueue, MiningQueue>();

            // Services
            services.AddSingleton<IBlockchainService, BlockchainService>();
            services.AddSingleton<ITransactionService, TransactionService>();
            services.AddSingleton<IMiningService, MiningService>();

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