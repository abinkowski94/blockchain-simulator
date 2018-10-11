using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Providers.Specific
{
    public class ProofOfWorkBlockProvider : BaseBlockProvider
    {
        public ProofOfWorkBlockProvider(IMerkleTreeProvider merkleTreeProvider,
            IConfigurationService configurationService) : base(merkleTreeProvider, configurationService)
        {
        }

        protected override async Task<BlockBase> FillBlock(BlockBase currentBlock, CancellationToken token)
        {
            currentBlock.Header.Version = _blockchainNodeConfiguration.Version;
            currentBlock.Header.Target = _blockchainNodeConfiguration.Target;
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