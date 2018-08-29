using Newtonsoft.Json;

namespace BlockchainSimulator.Node.DataAccess.Model.Block
{
    public class Block : BlockBase
    {
        [JsonProperty("isGenesis")] public override bool IsGenesis => false;
        [JsonProperty("parentId")] public string ParentId { get; set; }
    }
}