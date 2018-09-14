using Newtonsoft.Json;

namespace BlockchainSimulator.Node.WebApi.Models.Blockchain
{
    /// <summary>
    /// The encoded blocks
    /// </summary>
    public class EncodedBlocks
    {
        /// <summary>
        /// The blocks encoded in base 64 string
        /// </summary>
        [JsonProperty("base64Blocks")]
        public string Base64Blocks { get; set; }
    }
}