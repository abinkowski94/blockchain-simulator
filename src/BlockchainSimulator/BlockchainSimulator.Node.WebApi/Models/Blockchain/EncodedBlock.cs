using Newtonsoft.Json;

namespace BlockchainSimulator.Node.WebApi.Models.Blockchain
{
    /// <summary>
    /// The encoded blockchain
    /// </summary>
    public class EncodedBlock
    {
        /// <summary>
        /// The block encoded in base 64 string
        /// </summary>
        [JsonProperty("base64Block")]
        public string Base64Block { get; set; }
    }
}