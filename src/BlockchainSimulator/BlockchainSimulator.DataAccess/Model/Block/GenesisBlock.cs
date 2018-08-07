using Newtonsoft.Json;

namespace BlockchainSimulator.DataAccess.Model.Block
{
    public class GenesisBlock : BlockBase
    {
        [JsonProperty("isGenesis")]
        public override bool IsGenesis => true;
    }
}