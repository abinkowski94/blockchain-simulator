using Newtonsoft.Json;

namespace BlockchainSimulator.DataAccess.Model.Block
{
    public class Block : BlockBase
    {
        [JsonProperty("parentId")]
        public string ParentId { get; set; }
        
        [JsonProperty("isGenesis")]
        public override bool IsGenesis => false;
    }
}