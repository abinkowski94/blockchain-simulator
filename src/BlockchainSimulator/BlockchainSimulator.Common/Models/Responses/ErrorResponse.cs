using Newtonsoft.Json;

namespace BlockchainSimulator.Common.Models.Responses
{
    /// <inheritdoc />
    /// <summary>
    /// The error response
    /// </summary>
    public class ErrorResponse : BaseResponse
    {
        /// <summary>
        /// The error list
        /// </summary>
        [JsonProperty("errors", Order = 3)]
        public string[] Errors { get; set; }
    }
}