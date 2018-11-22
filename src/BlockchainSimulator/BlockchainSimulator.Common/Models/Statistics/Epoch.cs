using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlockchainSimulator.Common.Models.Statistics
{
    /// <summary>
    /// The epoch
    /// </summary>
    public class Epoch
    {
        /// <summary>
        /// Total stake
        /// </summary>
        [JsonProperty("totalStake")]
        public decimal TotalStake { get; set; }
        
        /// <summary>
        /// Finalized block id
        /// </summary>
        [JsonProperty("finalizedBlockId")]
        public string FinalizedBlockId { get; set; }
        
        /// <summary>
        /// Prepared block id
        /// </summary>
        [JsonProperty("preparedBlockId")]
        public string PreparedBlockId { get; set; }
        
        /// <summary>
        /// Has prepared
        /// </summary>
        [JsonProperty("hasPrepared")]
        public bool HasPrepared { get; set; }
        
        /// <summary>
        /// Has finalized
        /// </summary>
        [JsonProperty("hasFinalized")]
        public bool HasFinalized { get; set; }
        
        /// <summary>
        /// The epoch number
        /// </summary>
        [JsonProperty("number")]
        public int Number { get; set; }
        
        /// <summary>
        /// Checkpoints with prepare stakes
        /// </summary>
        [JsonProperty("checkpointsWithPrepareStakes")]
        public Dictionary<string, decimal> CheckpointsWithPrepareStakes { get; set; }
        
        /// <summary>
        /// Checkpoints with commit stakes
        /// </summary>
        [JsonProperty("checkpointsWithCommitStakes")]
        public Dictionary<string, decimal> CheckpointsWithCommitStakes { get; set; }
    }
}