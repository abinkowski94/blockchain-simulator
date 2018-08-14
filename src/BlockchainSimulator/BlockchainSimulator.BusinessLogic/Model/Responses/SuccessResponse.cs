namespace BlockchainSimulator.BusinessLogic.Model.Responses
{
    public class SuccessResponse<T> : BaseResponse<T>
    {
        public override bool IsSuccess => true;

        public SuccessResponse(string message, T result) : base(message, result)
        {
        }
    }
}