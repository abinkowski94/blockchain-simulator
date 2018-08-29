using Microsoft.Extensions.Configuration;

namespace BlockchainSimulator.Node.BusinessLogic.Configurations
{
    public class ProofOfWorkConfiguration : IBlockchainConfiguration
    {
        public int BlockSize { get; }
        public string Target { get; }
        public string Version { get; }

        public ProofOfWorkConfiguration(IConfiguration configuration)
        {
            Version = configuration["BlockchainConfiguration:Version"] ?? "PoW-v1";
            Target = configuration["BlockchainConfiguration:Target"] ?? "0000";
            BlockSize = int.TryParse(configuration["BlockchainConfiguration:BlockSize"], out var result) ? result : 10;
        }
    }
}