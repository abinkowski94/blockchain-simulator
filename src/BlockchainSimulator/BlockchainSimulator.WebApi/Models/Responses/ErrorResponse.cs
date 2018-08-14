namespace BlockchainSimulator.WebApi.Models.Responses
{
    public class ErrorResponse : BaseResponse
    {
        public string[] Errors { get; set; }
    }
}