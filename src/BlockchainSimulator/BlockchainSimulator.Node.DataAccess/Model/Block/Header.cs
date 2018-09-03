using Newtonsoft.Json;
using System;

namespace BlockchainSimulator.Node.DataAccess.Model.Block
{
    public class Header
    {
        [JsonProperty("merkleTreeRootHash", Order = 4)]
        public string MerkleTreeRootHash { get; set; }

        [JsonProperty("nonce", Order = 3)]
        public string Nonce { get; set; }

        [JsonProperty("parentHash", Order = 1)]
        public string ParentHash { get; set; }

        [JsonProperty("target", Order = 2)]
        public string Target { get; set; }

        [JsonProperty("timeStamp", Order = 5)]
        public DateTime TimeStamp { get; set; }

        [JsonProperty("version", Order = 6)]
        public string Version { get; set; }
    }
}