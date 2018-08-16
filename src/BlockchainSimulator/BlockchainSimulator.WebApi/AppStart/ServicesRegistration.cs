using BlockchainSimulator.BusinessLogic.Providers;
using BlockchainSimulator.BusinessLogic.Providers.Specific;
using BlockchainSimulator.BusinessLogic.Queue;
using BlockchainSimulator.BusinessLogic.Services;
using BlockchainSimulator.BusinessLogic.Services.Specific;
using BlockchainSimulator.BusinessLogic.Validators;
using BlockchainSimulator.BusinessLogic.Validators.Specific;
using BlockchainSimulator.DataAccess.Repositories;
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
        /// <returns>The service container</returns>
        public static IServiceCollection AddBlockchainServices(this IServiceCollection services)
        {
            // Repositories
            services.AddSingleton<IFileRepository, FileRepository>();
            services.AddSingleton<IBlockchainRepository, BlockchainRepository>();

            // Queues
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            // Services
            services.AddSingleton<IBlockchainService, BlockchainService>();
            services.AddSingleton<ITransactionService, TransactionService>();
            services.AddSingleton<IMiningService, MiningService>();
            services.AddSingleton<IConsensusService, ProofOfWorkConsensusService>(); // TODO: add configuration for it

            // Providers
            services.AddTransient<IMerkleTreeProvider, MerkleTreeProvider>();
            services.AddTransient<IBlockProvider, ProofOfWorkBlockProvider>(); // TODO: add configuration for it

            // Validators
            services.AddTransient<IMerkleTreeValidator, MerkleTreeValidator>();
            services.AddTransient<IBlockchainValidator, ProofOfWorkValidator>(); // TODO: add configuration for it

            return services;
        }
    }
}