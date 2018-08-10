using Newtonsoft.Json;

namespace BlockchainSimulator.WebApi.Models
{
    public class Transaction
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("sender")] 
        public string Sender { get; set; }

        [JsonProperty("recipient")]
        public string Recipient { get; set; }

        [JsonProperty("amount")] 
        public decimal Amount { get; set; }

        [JsonProperty("fee")] 
        public decimal Fee { get; set; }

        [JsonProperty("transactionDetails", NullValueHandling = NullValueHandling.Ignore)]
        public TransactionDetails TransactionDetails { get; set; }
    }
}