using Newtonsoft.Json;

namespace BlockchainSimulator.WebApi.Models
{
    public class TransactionDetails
    {
        [JsonProperty("isConfirmed")] 
        public bool IsConfirmed { get; set; }

        [JsonProperty("blockId")]
        public string BlockId { get; set; }

        [JsonProperty("blocksBehind")]
        public long BlocksBehind { get; set; }
    }
}