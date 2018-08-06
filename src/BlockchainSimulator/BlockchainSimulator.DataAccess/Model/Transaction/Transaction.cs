using Newtonsoft.Json;

namespace BlockchainSimulator.DataAccess.Model.Transaction
{
    public class Transaction
    {
        [JsonProperty("id")]
        public string Id { get; }
        
        [JsonProperty("sender")]
        public string Sender { get; }
        
        [JsonProperty("recipient")]
        public string Recipient { get; }
        
        [JsonProperty("amount")]
        public decimal Amount { get; }
        
        [JsonProperty("fee")]
        public decimal Fee { get; }

        public Transaction(string id, string sender, string recipient, decimal amount, decimal fee)
        {
            Id = id;
            Sender = sender;
            Recipient = recipient;
            Amount = amount;
            Fee = fee;
        }
    }
}