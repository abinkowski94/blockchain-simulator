using Newtonsoft.Json;

namespace BlockchainSimulator.BusinessLogic.Model.Block
{
    public abstract class BlockBase
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("body")]
        public Body Body { get; set; }

        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("isGenesis")]
        public abstract bool IsGenesis { get; }

        [JsonIgnore] 
        public string BlockJson => JsonConvert.SerializeObject(this);
    }
}