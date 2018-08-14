using Newtonsoft.Json;

namespace BlockchainSimulator.WebApi.Models
{
    public class ServerNode
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("httpAddress")]
        public string HttpAddress { get; set; }

        [JsonProperty("isConnected")]
        public bool? IsConnected { get; set; }
        
        [JsonProperty("delay")]
        public long Delay { get; set; }
    }
}