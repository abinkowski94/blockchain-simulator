using Newtonsoft.Json;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Block
{
    public class GenesisBlock : BlockBase
    {
        [JsonProperty("isGenesis")]
        public override bool IsGenesis => true;
    }
}