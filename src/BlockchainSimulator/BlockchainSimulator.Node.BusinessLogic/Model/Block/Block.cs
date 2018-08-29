using Newtonsoft.Json;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Block
{
    public class Block : BlockBase
    {
        [JsonProperty("isGenesis")]
        public override bool IsGenesis => false;

        [JsonIgnore]
        public BlockBase Parent { get; set; }

        [JsonProperty("parentId")]
        public string ParentId { get; set; }
    }
}