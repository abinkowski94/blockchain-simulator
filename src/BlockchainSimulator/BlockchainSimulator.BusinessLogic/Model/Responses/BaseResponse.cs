using System;

namespace BlockchainSimulator.BusinessLogic.Model.Responses
{
    public abstract class BaseResponse
    {
        public Guid Id { get; }
        public string Message { get; }
        public abstract bool IsSuccess { get; }

        protected BaseResponse(string message)
        {
            Message = message;
            Id = Guid.NewGuid();
        }
    }
}