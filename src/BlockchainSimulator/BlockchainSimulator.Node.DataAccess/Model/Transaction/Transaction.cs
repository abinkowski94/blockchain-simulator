using Newtonsoft.Json;
using System;

namespace BlockchainSimulator.Node.DataAccess.Model.Transaction
{
    public class Transaction
    {
        [JsonProperty("amount", Order = 4)]
        public decimal Amount { get; set; }
        
        [JsonProperty("fee", Order = 5)]
        public decimal Fee { get; set; }
        
        [JsonProperty("id", Order = 1)]
        public string Id { get; set; }

        [JsonProperty("recipient", Order = 3)]
        public string Recipient { get; set; }
        
        [JsonProperty("registrationTime", Order = 6)]
        public DateTime RegistrationTime { get; set; }
        
        [JsonProperty("sender", Order = 2)]
        public string Sender { get; set; }
    }
}