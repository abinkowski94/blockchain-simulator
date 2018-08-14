namespace BlockchainSimulator.BusinessLogic.Model.Responses
{
    public class ErrorResponse : BaseResponse
    {
        public override bool IsSuccess => false;
        public ErrorResponse[] InnerErrors { get; }

        public ErrorResponse(string message, ErrorResponse[] innerErrors) : base(message)
        {
            InnerErrors = innerErrors;
        }
    }
}