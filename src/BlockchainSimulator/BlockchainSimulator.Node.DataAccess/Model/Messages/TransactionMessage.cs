using Newtonsoft.Json;

namespace BlockchainSimulator.Node.DataAccess.Model.Messages
{
    public class TransactionMessage
    {
        [JsonProperty("messageType", Order = 1)]
        public TransactionMessageTypes MessageType { get; set; }
    }
}