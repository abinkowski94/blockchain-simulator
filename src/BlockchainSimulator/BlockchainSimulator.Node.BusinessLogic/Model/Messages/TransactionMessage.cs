using Newtonsoft.Json;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Messages
{
    public class TransactionMessage
    {
        [JsonProperty("messageType", Order = 1)]
        public TransactionMessageTypes MessageType { get; set; }
    }
}