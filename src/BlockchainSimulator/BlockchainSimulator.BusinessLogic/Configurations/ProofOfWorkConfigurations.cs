using Microsoft.Extensions.Configuration;

namespace BlockchainSimulator.BusinessLogic.Configurations
{
    public class ProofOfWorkConfiguration : IBlockchainConfiguration
    {
        public string Version { get; }
        public string Target { get; }
        public int BlockSize { get; }

        public ProofOfWorkConfiguration(IConfiguration configuration)
        {
            Version = configuration["Version"] ?? "PoW-v1";
            Target = configuration["Target"] ?? "0000";
            BlockSize = int.TryParse(configuration["BlockSize"], out var result) ? result : 10;
        }
    }
}