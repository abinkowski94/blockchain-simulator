using Newtonsoft.Json;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Block
{
    public class Block : BlockBase
    {
        private int? _depth;

        [JsonIgnore]
        public override int Depth
        {
            get => _depth ?? 1 + (Parent as Block)?.Depth ?? 1;
            set => _depth = value;
        }

        [JsonIgnore]
        public BlockBase Parent { get; set; }

        [JsonProperty("isGenesis")]
        public override bool IsGenesis => false;

        [JsonProperty("parentId")]
        public string ParentUniqueId { get; set; }
    }
}