using BlockchainSimulator.Node.BusinessLogic.Configurations;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Services;
using System;
using Microsoft.Extensions.Configuration;

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

        protected override BlockBase FillBlock(BlockBase currentBlock)
        {
            currentBlock.Header.Version = _blockchainConfiguration.Version;
            currentBlock.Header.Target = _blockchainConfiguration.Target;
            currentBlock.Header.Nonce = GetProof(currentBlock);

            return currentBlock;
        }

        private static string GetProof(BlockBase block)
        {
            long expectedNonce = 1;
            block.Header.Nonce = Convert.ToString(expectedNonce, 16);

            while (!EncryptionService.GetSha256Hash(block.BlockJson).StartsWith(block.Header.Target))
            {
                expectedNonce++;
                block.Header.Nonce = Convert.ToString(expectedNonce, 16);
            }

            return Convert.ToString(expectedNonce, 16);
        }
    }
}