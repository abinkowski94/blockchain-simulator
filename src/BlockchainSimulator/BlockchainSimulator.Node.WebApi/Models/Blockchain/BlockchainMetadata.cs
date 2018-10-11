using Newtonsoft.Json;

namespace BlockchainSimulator.Node.WebApi.Models.Blockchain
{
    /// <summary>
    /// The blockchain meta-data
    /// </summary>
    public class BlockchainMetadata
    {
        /// <summary>
        /// The length of the blockchain
        /// </summary>
        [JsonProperty("nodes", Order = 1)]
        public int Nodes { get; set; }
    }
}