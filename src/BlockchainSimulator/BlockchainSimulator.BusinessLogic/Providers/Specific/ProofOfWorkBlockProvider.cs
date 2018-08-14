using System;
using BlockchainSimulator.BusinessLogic.Configurations;
using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Services;

namespace BlockchainSimulator.BusinessLogic.Providers.Specific
{
    public class ProofOfWorkBlockProvider : BaseBlockProvider
    {
        public ProofOfWorkBlockProvider(IMerkleTreeProvider merkleTreeProvider) : base(merkleTreeProvider)
        {
        }

        protected override BlockBase FillBlock(BlockBase currentBlock)
        {
            currentBlock.Header.Version = ProofOfWorkConfigurations.Version;
            currentBlock.Header.Target = ProofOfWorkConfigurations.Target;
            currentBlock.Header.Nonce = GetProof(currentBlock);

            return currentBlock;
        }

        private static string GetProof(BlockBase block)
        {
            long expectedNonce = 0;

            while (!EncryptionService.GetSha256Hash(block.BlockJson).StartsWith(block.Header.Target))
            {
                expectedNonce++;
                block.Header.Nonce = Convert.ToString(expectedNonce, 16);
            }

            return Convert.ToString(expectedNonce, 16);
        }
    }
}