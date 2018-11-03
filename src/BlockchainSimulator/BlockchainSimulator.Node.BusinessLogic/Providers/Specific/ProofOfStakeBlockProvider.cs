using System.Threading;
using System.Threading.Tasks;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Services;

namespace BlockchainSimulator.Node.BusinessLogic.Providers.Specific
{
    public class ProofOfStakeBlockProvider : BaseBlockProvider
    {
        public ProofOfStakeBlockProvider(IMerkleTreeProvider merkleTreeProvider,
            IConfigurationService configurationService) : base(merkleTreeProvider, configurationService)
        {
        }

        protected override Task<BlockBase> FillBlock(BlockBase currentBlock, CancellationToken token)
        {
            return Task.Run(() =>
            {
                token.ThrowIfCancellationRequested();

                currentBlock.Header.Version = BlockchainNodeConfiguration.Version;
                currentBlock.Header.Target = null;
                currentBlock.Header.Nonce = null;
                
                

                return currentBlock;
            }, token);
        }
    }
}