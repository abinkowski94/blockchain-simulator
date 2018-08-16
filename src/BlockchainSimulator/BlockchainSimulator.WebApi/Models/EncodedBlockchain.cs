using Newtonsoft.Json;

namespace BlockchainSimulator.WebApi.Models
{
    /// <summary>
    /// The encoded blockchain
    /// </summary>
    public class EncodedBlockchain
    {
        /// <summary>
        /// The blockchain encoded in base 64 string
        /// </summary>
        [JsonProperty("base64Blockchain")]
        public string Base64Blockchain { get; set; }
    }
}