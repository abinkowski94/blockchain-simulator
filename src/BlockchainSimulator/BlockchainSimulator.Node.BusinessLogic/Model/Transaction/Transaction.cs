using System;
using Newtonsoft.Json;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Transaction
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
        
        [JsonProperty("registrationTime")]
        public DateTime RegistrationTime { get; set; }

        [JsonIgnore] 
        public string TransactionJson => JsonConvert.SerializeObject(this);

        [JsonIgnore]
        public TransactionDetails TransactionDetails { get; set; }
    }
}