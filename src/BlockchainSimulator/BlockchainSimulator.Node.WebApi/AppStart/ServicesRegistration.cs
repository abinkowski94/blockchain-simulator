using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Common.Services;
using BlockchainSimulator.Node.BusinessLogic.Providers;
using BlockchainSimulator.Node.BusinessLogic.Providers.Specific;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using BlockchainSimulator.Node.BusinessLogic.Services;
using BlockchainSimulator.Node.BusinessLogic.Services.Specific;
using BlockchainSimulator.Node.BusinessLogic.Storage;
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
            services.AddSingleton<IBackgroundQueue, BackgroundQueue>();

            services.AddHostedService<MiningHostedService>();
            services.AddHostedService<QueuedHostedService>();
            services.AddHostedService<ReMiningHostedService>();

            // Services and storage
            services.AddSingleton<IConfigurationService, ConfigurationService>();
            services.AddSingleton<IStatisticService, StatisticService>();

            services.AddSingleton<IStakingStorage, StakingStorage>();
            services.AddSingleton<IEncodedBlocksStorage, EncodedBlocksStorage>();
            services.AddSingleton<IServerNodesStorage, ServerNodesStorage>();
            services.AddSingleton<ITransactionStorage, TransactionStorage>();

            services.AddTransient<ITransactionService, TransactionService>();
            services.AddTransient<IConsensusService, ConsensusService>();
            services.AddTransient<IMiningService, MiningService>();
            services.AddTransient<IHttpService, HttpService>();

            // Providers
            services.AddTransient<IMerkleTreeProvider, MerkleTreeProvider>();

            // Validators
            services.AddTransient<IMerkleTreeValidator, MerkleTreeValidator>();

            switch (configuration["Node:Type"])
            {
                case "PoW":
                    // Services
                    services.AddTransient<IBlockchainService, ProofOfWorkBlockchainService>();

                    // Providers
                    services.AddTransient<IBlockProvider, ProofOfWorkBlockProvider>();

                    // Validators
                    services.AddTransient<IBlockchainValidator, ProofOfWorkValidator>();

                    break;
                case "PoS":
                    // Services and storage
                    services.AddSingleton<IBlockchainService, ProofOfStakeBlockchainService>();

                    // Providers
                    services.AddTransient<IBlockProvider, ProofOfStakeBlockProvider>();

                    // Validators
                    services.AddTransient<IBlockchainValidator, ProofOfStakeValidator>();

                    break;
            }

            return services;
        }
    }
}