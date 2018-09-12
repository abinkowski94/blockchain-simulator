using Newtonsoft.Json;

namespace BlockchainSimulator.Node.DataAccess.Model
{
    public class BlockchainTreeMetadata
    {
        [JsonProperty("nodes", Order = 1)]
        public int Nodes { get; set; }
    }
}