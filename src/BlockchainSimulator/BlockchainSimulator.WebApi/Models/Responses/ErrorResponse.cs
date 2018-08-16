namespace BlockchainSimulator.WebApi.Models.Responses
{
    /// <summary>
    /// The error response
    /// </summary>
    public class ErrorResponse : BaseResponse
    {
        /// <summary>
        /// The error list
        /// </summary>
        public string[] Errors { get; set; }
    }
}