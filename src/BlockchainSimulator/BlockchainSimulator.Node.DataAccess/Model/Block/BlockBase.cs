using Newtonsoft.Json;
using System;

namespace BlockchainSimulator.Node.DataAccess.Model.Block
{
    public abstract class BlockBase
    {
        [JsonProperty("body", Order = 6)]
        public Body Body { get; set; }

        [JsonProperty("header", Order = 5)]
        public Header Header { get; set; }

        [JsonProperty("id", Order = 1)]
        public string Id { get; set; }

        [JsonProperty("isGenesis", Order = 2)]
        public abstract bool IsGenesis { get; }

        [JsonProperty("queueTime", Order = 4)]
        public TimeSpan QueueTime { set; get; }
    }
}