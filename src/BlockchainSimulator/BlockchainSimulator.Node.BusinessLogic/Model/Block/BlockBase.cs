using Newtonsoft.Json;
using System;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Block
{
    public abstract class BlockBase
    {
        [JsonIgnore]
        public string BlockJson => JsonConvert.SerializeObject(this);

        [JsonIgnore]
        public abstract int Depth { get; set; }

        [JsonProperty("body")]
        public Body Body { get; set; }

        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("uniqueId")]
        public string UniqueId { get; set; }

        [JsonProperty("isGenesis")]
        public abstract bool IsGenesis { get; }

        [JsonProperty("queueTime")]
        public TimeSpan QueueTime { set; get; }
    }
}