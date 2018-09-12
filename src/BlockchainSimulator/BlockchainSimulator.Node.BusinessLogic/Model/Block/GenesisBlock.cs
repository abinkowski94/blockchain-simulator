using Newtonsoft.Json;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Block
{
    public class GenesisBlock : BlockBase
    {
        [JsonIgnore]
        public override int Depth
        {
            get => 0;
            set { }
        }

        [JsonProperty("isGenesis")] public override bool IsGenesis => true;
    }
}