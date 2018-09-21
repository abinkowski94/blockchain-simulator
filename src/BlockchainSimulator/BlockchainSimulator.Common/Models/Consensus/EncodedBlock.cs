using Newtonsoft.Json;
using System.Collections.Generic;

namespace BlockchainSimulator.Common.Models.Consensus
{
    /// <summary>
    /// The encoded block
    /// </summary>
    public class EncodedBlock
    {
        /// <summary>
        /// The id of the encoded block
        /// </summary>
        [JsonProperty]
        public string Id { get; set; }

        /// <summary>
        /// The block encoded in base 64 string
        /// </summary>
        [JsonProperty("base64Block")]
        public string Base64Block { get; set; }

        /// <summary>
        /// The node sender id
        /// </summary>
        [JsonProperty("nodeSenderId")]
        public string NodeSenderId { get; set; }

        /// <summary>
        /// The nodes accepted ids
        /// </summary>
        [JsonProperty("nodesAcceptedIds")]
        public List<string> NodesAcceptedIds { get; set; }
    }
}