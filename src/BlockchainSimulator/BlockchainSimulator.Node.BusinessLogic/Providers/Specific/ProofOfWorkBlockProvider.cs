using System;
using BlockchainSimulator.Node.BusinessLogic.Configurations;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Services;

namespace BlockchainSimulator.Node.BusinessLogic.Providers.Specific
{
    public class ProofOfWorkBlockProvider : BaseBlockProvider
    {
        private readonly IBlockchainConfiguration _configuration;

        public ProofOfWorkBlockProvider(IMerkleTreeProvider merkleTreeProvider, IBlockchainConfiguration configuration)
            : base(merkleTreeProvider)
        {
            _configuration = configuration;
        }

        protected override BlockBase FillBlock(BlockBase currentBlock)
        {
            currentBlock.Header.Version = _configuration.Version;
            currentBlock.Header.Target = _configuration.Target;
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