using Newtonsoft.Json;
using System;

namespace BlockchainSimulator.Common.Models.Statistics
{
    /// <summary>
    /// The transactions statistics
    /// </summary>
    public class TransactionStatistics
    {
        /// <summary>
        /// The id of the block
        /// </summary>
        [JsonProperty("blockId", Order = 1)]
        public string BlockId { get; set; }

        /// <summary>
        /// The block queue time
        /// </summary>
        [JsonProperty("blockQueueTime", Order = 2)]
        public TimeSpan BlockQueueTime { get; set; }

        /// <summary>
        /// The transaction confirmation time
        /// </summary>
        [JsonProperty("transactionConfirmationTime", Order = 6)]
        public TimeSpan TransactionConfirmationTime { get; set; }

        /// <summary>
        /// The transaction fee
        /// </summary>
        [JsonProperty("transactionFee", Order = 4)]
        public decimal TransactionFee { get; set; }

        /// <summary>
        /// The transaction id
        /// </summary>
        [JsonProperty("transactionId", Order = 3)]
        public string TransactionId { get; set; }

        /// <summary>
        /// The transaction registration time
        /// </summary>
        [JsonProperty("transactionRegistrationTime", Order = 5)]
        public DateTime TransactionRegistrationTime { get; set; }
    }
}