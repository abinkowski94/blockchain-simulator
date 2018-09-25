using BlockchainSimulator.Node.BusinessLogic.Configurations;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Providers.Specific
{
    public class ProofOfWorkBlockProvider : BaseBlockProvider
    {
        private readonly IBlockchainConfiguration _blockchainConfiguration;

        public ProofOfWorkBlockProvider(IMerkleTreeProvider merkleTreeProvider,
            IBlockchainConfiguration blockchainConfiguration, IConfiguration configuration)
            : base(merkleTreeProvider, configuration)
        {
            _blockchainConfiguration = blockchainConfiguration;
        }

        protected override async Task<BlockBase> FillBlock(BlockBase currentBlock, CancellationToken token)
        {
            currentBlock.Header.Version = _blockchainConfiguration.Version;
            currentBlock.Header.Target = _blockchainConfiguration.Target;
            currentBlock.Header.Nonce = await GetProofAsync(currentBlock, token);

            return currentBlock;
        }

        private static Task<string> GetProofAsync(BlockBase block, CancellationToken token)
        {
            return Task.Run(() =>
            {
                long expectedNonce = 1;
                block.Header.Nonce = Convert.ToString(expectedNonce, 16);

                while (!EncryptionService.GetSha256Hash(block.BlockJson).StartsWith(block.Header.Target))
                {
                    expectedNonce++;
                    block.Header.Nonce = Convert.ToString(expectedNonce, 16);
                    token.ThrowIfCancellationRequested();
                }

                return Convert.ToString(expectedNonce, 16);
            }, token);
        }
    }
}