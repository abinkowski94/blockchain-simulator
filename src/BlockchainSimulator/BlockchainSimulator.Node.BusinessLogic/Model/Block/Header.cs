using System;
using Newtonsoft.Json;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Block
{
    public class Header
    {
        [JsonProperty("version")] 
        public string Version { get; set; }

        [JsonProperty("parentHash")] 
        public string ParentHash { get; set; }

        [JsonProperty("merkleTreeRootHash")] 
        public string MerkleTreeRootHash { get; set; }

        [JsonProperty("timeStamp")] 
        public DateTime TimeStamp { get; set; }

        [JsonProperty("target")] 
        public string Target { get; set; }

        [JsonProperty("nonce")]
        public string Nonce { get; set; }
    }
}