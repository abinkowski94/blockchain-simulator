using Newtonsoft.Json;

namespace BlockchainSimulator.Node.DataAccess.Model.Block
{
    public class GenesisBlock : BlockBase
    {
        [JsonProperty("isGenesis", Order = 4)]
        public override bool IsGenesis => true;
    }
}