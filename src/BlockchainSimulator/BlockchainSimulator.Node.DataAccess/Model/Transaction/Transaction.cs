using Newtonsoft.Json;
using System;
using BlockchainSimulator.Node.DataAccess.Model.Messages;

namespace BlockchainSimulator.Node.DataAccess.Model.Transaction
{
    public class Transaction
    {
        [JsonProperty("id", Order = 1)]
        public string Id { get; set; }

        [JsonProperty("sender", Order = 2)]
        public string Sender { get; set; }

        [JsonProperty("recipient", Order = 3)]
        public string Recipient { get; set; }

        [JsonProperty("amount", Order = 4)] 
        public decimal Amount { get; set; }

        [JsonProperty("fee", Order = 5)] 
        public decimal Fee { get; set; }

        [JsonProperty("registrationTime", Order = 6)]
        public DateTime RegistrationTime { get; set; }

        [JsonProperty("transactionMessage", Order = 7)] 
        public TransactionMessage TransactionMessage { get; set; }
    }
}