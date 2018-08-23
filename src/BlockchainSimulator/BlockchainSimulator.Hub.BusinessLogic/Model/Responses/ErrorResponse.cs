namespace BlockchainSimulator.Hub.BusinessLogic.Model.Responses
{
    public class ErrorResponse<T> : BaseResponse<T>
    {
        public override bool IsSuccess => false;
        public string[] Errors { get; }
        
        public ErrorResponse(string message, T result, params string[] errors) : base(message, result)
        {
            Errors = errors;
        }
    }
}