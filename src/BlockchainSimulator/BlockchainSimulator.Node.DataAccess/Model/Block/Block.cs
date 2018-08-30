using Newtonsoft.Json;

namespace BlockchainSimulator.Node.DataAccess.Model.Block
{
    public class Block : BlockBase
    {
        [JsonProperty("isGenesis", Order = 2)]
        public override bool IsGenesis => false;
        
        [JsonProperty("parentId", Order = 3)]
        public string ParentId { get; set; }
    }
}