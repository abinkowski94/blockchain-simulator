using Newtonsoft.Json;
using System;

namespace BlockchainSimulator.Node.DataAccess.Model.Block
{
    public abstract class BlockBase
    {
        [JsonProperty("body")] public Body Body { get; set; }
        [JsonProperty("header")] public Header Header { get; set; }
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("isGenesis")] public abstract bool IsGenesis { get; }
        [JsonProperty("queueTime")] public TimeSpan QueueTime { set; get; }
    }
}