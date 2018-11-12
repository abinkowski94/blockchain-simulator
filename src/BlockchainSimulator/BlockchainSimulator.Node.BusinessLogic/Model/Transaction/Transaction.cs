using Newtonsoft.Json;
using System;
using BlockchainSimulator.Node.BusinessLogic.Model.Messages;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Transaction
{
    public class Transaction
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("fee")]
        public decimal Fee { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("recipient")]
        public string Recipient { get; set; }

        [JsonProperty("registrationTime")]
        public DateTime? RegistrationTime { get; set; }

        [JsonProperty("sender")]
        public string Sender { get; set; }

        [JsonProperty("transactionMessage")]
        public TransactionMessage TransactionMessage { get; set; }
        
        [JsonIgnore]
        public TransactionDetails TransactionDetails { get; set; }

        [JsonIgnore]
        public string TransactionJson => JsonConvert.SerializeObject(this);
    }
}