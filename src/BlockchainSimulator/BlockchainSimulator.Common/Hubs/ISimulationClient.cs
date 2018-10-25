using System.Threading.Tasks;

namespace BlockchainSimulator.Common.Hubs
{
    /// <summary>
    /// The simulation client contract
    /// </summary>
    public interface ISimulationClient
    {
        /// <summary>
        /// Changes working status of the node
        /// </summary>
        /// <param name="isWorking">Flag if the node is working</param>
        /// <returns>The working status</returns>
        Task ChangeWorkingStatus(bool isWorking);
    }
}