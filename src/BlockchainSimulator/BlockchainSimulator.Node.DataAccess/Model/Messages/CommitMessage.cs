using Newtonsoft.Json;

namespace BlockchainSimulator.Node.DataAccess.Model.Messages
{
    public class CommitMessage : TransactionMessage
    {
        [JsonProperty("epochTarget", Order = 2)]
        public int EpochTarget { get; set; }

        [JsonProperty("idTarget", Order = 3)]
        public string IdTarget { get; set; }
    }
}