using Newtonsoft.Json;

namespace BlockchainSimulator.Node.DataAccess.Model.Block
{
    public class Block : BlockBase
    {
        [JsonProperty("isGenesis", Order = 4)] 
        public override bool IsGenesis => false;

        [JsonProperty("parentUniqueId", Order = 2)]
        public string ParentUniqueId { get; set; }
    }
}