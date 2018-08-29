using Newtonsoft.Json;
using System;

namespace BlockchainSimulator.Node.DataAccess.Model.Transaction
{
    public class Transaction
    {
        [JsonProperty("amount")] public decimal Amount { get; set; }
        [JsonProperty("fee")] public decimal Fee { get; set; }
        [JsonProperty("id")] public string Id { get; set; }

        [JsonProperty("recipient")] public string Recipient { get; set; }
        [JsonProperty("registrationTime")] public DateTime RegistrationTime { get; set; }
        [JsonProperty("sender")] public string Sender { get; set; }
    }
}