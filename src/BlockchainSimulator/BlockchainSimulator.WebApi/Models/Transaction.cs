using Newtonsoft.Json;

namespace BlockchainSimulator.WebApi.Models
{
    /// <summary>
    /// The transaction
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Amount of coins sent
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// The fee
        /// </summary>
        [JsonProperty("fee")]
        public decimal Fee { get; set; }

        /// <summary>
        /// Id of the transaction
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The recipient address
        /// </summary>
        [JsonProperty("recipient")]
        public string Recipient { get; set; }

        /// <summary>
        /// The sender address
        /// </summary>
        [JsonProperty("sender")]
        public string Sender { get; set; }

        /// <summary>
        /// The transaction details
        /// </summary>
        [JsonProperty("transactionDetails", NullValueHandling = NullValueHandling.Ignore)]
        public TransactionDetails TransactionDetails { get; set; }
    }
}