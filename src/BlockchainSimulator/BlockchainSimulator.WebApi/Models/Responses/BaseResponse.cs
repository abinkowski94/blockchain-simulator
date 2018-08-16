using System;
using Newtonsoft.Json;

namespace BlockchainSimulator.WebApi.Models.Responses
{
    /// <summary>
    /// The base response
    /// </summary>
    public abstract class BaseResponse
    {
        /// <summary>
        /// Id of the response (used for event tracking)
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// The message of the response
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// The result of the method
        /// </summary>
        [JsonProperty("result")]
        public object Result { get; set; }
    }
}