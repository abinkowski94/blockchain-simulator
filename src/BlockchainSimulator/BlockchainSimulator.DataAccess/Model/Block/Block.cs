using Newtonsoft.Json;

namespace BlockchainSimulator.DataAccess.Model.Block
{
    public class Block : BlockBase
    {
        [JsonProperty("parentId")]
        public string ParentId { get; }
        
        [JsonProperty("isGenesis")]
        public override bool IsGenesis => false;

        public Block(Body body, Header header, string id, string parentId) : base(body, header, id)
        {
            ParentId = parentId;
        }
    }
}