using System;
using Newtonsoft.Json;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Block
{
    public abstract class BlockBase
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("body")]
        public Body Body { get; set; }

        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("queueTime")] 
        public TimeSpan QueueTime { set; get; }
        
        [JsonProperty("isGenesis")]
        public abstract bool IsGenesis { get; }

        [JsonIgnore] 
        public string BlockJson => JsonConvert.SerializeObject(this);
    }
}