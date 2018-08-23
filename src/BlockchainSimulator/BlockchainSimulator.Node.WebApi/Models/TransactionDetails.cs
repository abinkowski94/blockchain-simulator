using Newtonsoft.Json;

namespace BlockchainSimulator.Node.WebApi.Models
{
    /// <summary>
    /// The transaction details
    /// </summary>
    public class TransactionDetails
    {
        /// <summary>
        /// The block id which the transaction is in
        /// </summary>
        [JsonProperty("blockId")]
        public string BlockId { get; set; }

        /// <summary>
        /// Number of blocks behind
        /// </summary>
        [JsonProperty("blocksBehind")]
        public long BlocksBehind { get; set; }

        /// <summary>
        /// Is transaction confirmed
        /// </summary>
        [JsonProperty("isConfirmed")]
        public bool IsConfirmed { get; set; }
    }
}