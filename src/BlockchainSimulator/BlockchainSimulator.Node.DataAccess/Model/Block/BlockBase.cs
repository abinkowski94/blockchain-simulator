using Newtonsoft.Json;
using System;

namespace BlockchainSimulator.Node.DataAccess.Model.Block
{
    public abstract class BlockBase
    {
        [JsonProperty("body", Order = 7)]
        public Body Body { get; set; }

        [JsonProperty("header", Order = 6)]
        public Header Header { get; set; }

        [JsonProperty("id", Order = 3)]
        public string Id { get; set; }
        
        [JsonProperty("uniqueId", Order = 1)]
        public string UniqueId { get; set; }

        [JsonProperty("isGenesis", Order = 4)]
        public abstract bool IsGenesis { get; }

        [JsonProperty("queueTime", Order = 5)]
        public TimeSpan QueueTime { set; get; }
    }
}