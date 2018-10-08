using BlockchainSimulator.Common.Models.Consensus;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Hubs
{
    public interface IConsensusClient
    {
        Task ReceiveBlock(EncodedBlock encodedBlock);
    }
}