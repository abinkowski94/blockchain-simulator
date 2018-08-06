using System;
using Newtonsoft.Json;

namespace BlockchainSimulator.DataAccess.Model.Block
{
    public class Header
    {
        [JsonProperty("version")]
        public string Version { get; }
        
        [JsonProperty("parentHash")]
        public string ParentHash { get; }
        
        [JsonProperty("merkleTreeRootHash")]
        public string MerkleTreeRootHash { get; }
        
        [JsonProperty("timeStamp")]
        public DateTime TimeStamp { get; }
        
        [JsonProperty("target")]
        public string Target { get; }
        
        [JsonProperty("nonce")]
        public string Nonce { get; }
        
        public Header(string version, string parentHash, string merkleTreeRootHash, DateTime timeStamp, string target,
            string nonce)
        {
            TimeStamp = timeStamp;
            Version = version;
            ParentHash = parentHash;
            MerkleTreeRootHash = merkleTreeRootHash;
            Target = target;
            Nonce = nonce;
        }
    }
}