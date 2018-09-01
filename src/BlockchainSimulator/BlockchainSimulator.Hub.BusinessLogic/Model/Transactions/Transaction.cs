using Newtonsoft.Json;

namespace BlockchainSimulator.Hub.BusinessLogic.Model.Transactions
{
    public class Transaction
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("fee")]
        public decimal Fee { get; set; }

        [JsonProperty("recipient")]
        public string Recipient { get; set; }

        [JsonProperty("sender")]
        public string Sender { get; set; }
    }
}