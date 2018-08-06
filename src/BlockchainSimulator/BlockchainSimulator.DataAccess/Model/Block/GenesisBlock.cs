using Newtonsoft.Json;

namespace BlockchainSimulator.DataAccess.Model.Block
{
    public class GenesisBlock : BlockBase
    {
        [JsonProperty("isGenesis")]
        public override bool IsGenesis => true;

        public GenesisBlock(Body body, Header header, string id) : base(body, header, id)
        {
        }
    }
}