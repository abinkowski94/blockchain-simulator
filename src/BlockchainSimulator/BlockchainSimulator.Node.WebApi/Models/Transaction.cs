using Newtonsoft.Json;
using System;

namespace BlockchainSimulator.Node.WebApi.Models
{
    /// <summary>
    /// The transaction
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Amount of coins sent
        /// </summary>
        [JsonProperty("amount", Order = 4)]
        public decimal Amount { get; set; }

        /// <summary>
        /// The fee
        /// </summary>
        [JsonProperty("fee", Order = 5)]
        public decimal Fee { get; set; }

        /// <summary>
        /// Id of the transaction
        /// </summary>
        [JsonProperty("id", Order = 1)]
        public string Id { get; set; }

        /// <summary>
        /// The recipient address
        /// </summary>
        [JsonProperty("recipient", Order = 3)]
        public string Recipient { get; set; }

        /// <summary>
        /// The time when the transaction has been registered
        /// </summary>
        [JsonProperty("registrationTime", Order = 6)]
        public DateTime RegistrationTime { get; set; }

        /// <summary>
        /// The sender address
        /// </summary>
        [JsonProperty("sender", Order = 2)]
        public string Sender { get; set; }

        /// <summary>
        /// The transaction details
        /// </summary>
        [JsonProperty("transactionDetails", Order = 7, NullValueHandling = NullValueHandling.Ignore)]
        public TransactionDetails TransactionDetails { get; set; }
    }
}