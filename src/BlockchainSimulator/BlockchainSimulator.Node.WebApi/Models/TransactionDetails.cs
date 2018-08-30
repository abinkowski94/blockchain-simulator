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
        [JsonProperty("blockId", Order = 1)]
        public string BlockId { get; set; }

        /// <summary>
        /// Number of blocks behind
        /// </summary>
        [JsonProperty("blocksBehind", Order = 3)]
        public long BlocksBehind { get; set; }

        /// <summary>
        /// Is transaction confirmed
        /// </summary>
        [JsonProperty("isConfirmed", Order = 2)]
        public bool IsConfirmed { get; set; }
    }
}