using Newtonsoft.Json;

namespace BlockchainSimulator.WebApi.Models
{
    public class EncodedBlockchain
    {
        [JsonProperty("base64Blockchain")]
        public string Base64Blockchain { get; set; }
    }
}