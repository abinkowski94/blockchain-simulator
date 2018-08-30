using Newtonsoft.Json;
using System;

namespace BlockchainSimulator.Common.Models.Responses
{
    /// <summary>
    /// The base response
    /// </summary>
    public abstract class BaseResponse
    {
        /// <summary>
        /// Id of the response (used for event tracking)
        /// </summary>
        [JsonProperty("id", Order = 1)]
        public Guid Id { get; set; }

        /// <summary>
        /// The message of the response
        /// </summary>
        [JsonProperty("message", Order = 2)]
        public string Message { get; set; }

        /// <summary>
        /// The result of the method
        /// </summary>
        [JsonProperty("result", Order = 4)]
        public object Result { get; set; }
    }
}