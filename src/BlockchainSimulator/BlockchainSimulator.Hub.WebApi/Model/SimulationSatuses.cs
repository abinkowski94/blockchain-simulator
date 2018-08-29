namespace BlockchainSimulator.Hub.WebApi.Model
{
    /// <summary>
    /// The
    /// </summary>
    public enum SimulationStatuses
    {
        /// <summary>
        /// The simulation is ready to run
        /// </summary>
        ReadyToRun = 0,

        /// <summary>
        /// The simulation is in queue
        /// </summary>
        Pending = 1,

        /// <summary>
        /// The simulation is
        /// </summary>
        Preparing = 2,

        /// <summary>
        /// The simulation is running
        /// </summary>
        Running = 3,

        /// <summary>
        /// Waiting for network
        /// </summary>
        WaitingForNetwork = 4,

        /// <summary>
        /// Waiting for statistics to generate
        /// </summary>
        WaitingForStatistics = 5
    }
}