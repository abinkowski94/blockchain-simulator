using Newtonsoft.Json;

namespace BlockchainSimulator.Node.DataAccess.Model.Messages
{
    public class PrepareMessage : TransactionMessage
    {
        [JsonProperty("epochSource", Order = 2)]
        public int EpochSource { get; set; }

        [JsonProperty("epochTarget", Order = 3)]
        public int EpochTarget { get; set; }

        [JsonProperty("idSource", Order = 4)] 
        public string IdSource { get; set; }

        [JsonProperty("idTarget", Order = 5)]
        public string IdTarget { get; set; }
    }
}