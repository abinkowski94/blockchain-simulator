namespace BlockchainSimulator.BusinessLogic.Model.Responses
{
    public class SuccessResponse<T> : BaseResponse
    {
        public T Result { get; }
        public override bool IsSuccess => true;

        public SuccessResponse(string message, T result) : base(message)
        {
            Result = result;
        }
    }
}