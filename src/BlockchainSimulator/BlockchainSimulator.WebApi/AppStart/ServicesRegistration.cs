using BlockchainSimulator.BusinessLogic.Providers;
using BlockchainSimulator.BusinessLogic.Providers.Specific;
using BlockchainSimulator.BusinessLogic.Services;
using BlockchainSimulator.BusinessLogic.Validators;
using BlockchainSimulator.BusinessLogic.Validators.Specific;
using BlockchainSimulator.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BlockchainSimulator.WebApi.AppStart
{
    public static class ServicesRegistration
    {
        public static IServiceCollection AddBlockchainServices(this IServiceCollection services)
        {
            // Repositories
            services.AddSingleton<IFileRepository, FileRepository>();
            services.AddSingleton<IBlockchainRepository, BlockchainRepository>();

            // Services, Providers, Validators
            services.AddTransient<IEncryptionService, EncryptionService>();
            services.AddSingleton<IBlockchainService, BlockchainService>();
            services.AddSingleton<ITransactionService, TransactionService>();

            services.AddTransient<IMerkleTreeProvider, MerkleTreeProvider>();
            services.AddTransient<IBlockProvider, ProofOfWorkBlockProvider>(); // TODO: add configuration fo it

            services.AddTransient<IMerkleTreeValidator, MerkleTreeValidator>();
            services.AddTransient<IBlockchainValidator, ProofOfWorkValidator>(); // TODO: add configuration for it

            return services;
        }
    }
}