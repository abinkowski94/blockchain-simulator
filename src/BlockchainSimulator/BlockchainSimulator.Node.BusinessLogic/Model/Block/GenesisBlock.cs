using Newtonsoft.Json;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Block
{
    public class GenesisBlock : BlockBase
    {
        [JsonIgnore] 
        public override int Depth => 0;

        [JsonProperty("isGenesis")] 
        public override bool IsGenesis => true;
    }
}