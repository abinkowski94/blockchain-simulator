using Newtonsoft.Json;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Block
{
    public class Block : BlockBase
    {
        [JsonIgnore] 
        public int Depth => 1 + (Parent as Block)?.Depth ?? 1;

        [JsonProperty("isGenesis")] 
        public override bool IsGenesis => false;

        [JsonIgnore] 
        public BlockBase Parent { get; set; }

        [JsonProperty("parentId")] 
        public string ParentUniqueId { get; set; }
    }
}