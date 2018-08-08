using Newtonsoft.Json;

namespace BlockchainSimulator.BusinessLogic.Model.Block
{
    public class GenesisBlock : BlockBase
    {
        [JsonProperty("isGenesis")]
        public override bool IsGenesis => true;
    }
}