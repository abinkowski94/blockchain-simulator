using Newtonsoft.Json;
using System;

namespace BlockchainSimulator.Node.DataAccess.Model.Block
{
    public class Header
    {
        [JsonProperty("merkleTreeRootHash")] public string MerkleTreeRootHash { get; set; }
        [JsonProperty("nonce")] public string Nonce { get; set; }
        [JsonProperty("parentHash")] public string ParentHash { get; set; }
        [JsonProperty("target")] public string Target { get; set; }
        [JsonProperty("timeStamp")] public DateTime TimeStamp { get; set; }
        [JsonProperty("version")] public string Version { get; set; }
    }
}