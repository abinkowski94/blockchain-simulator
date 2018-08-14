using System;

namespace BlockchainSimulator.BusinessLogic.Model.Responses
{
    public abstract class BaseResponse<T>
    {
        public Guid Id { get; }
        public string Message { get; }
        public T Result { get; }
        public abstract bool IsSuccess { get; }

        protected BaseResponse(string message, T result)
        {
            Message = message;
            Result = result;
            Id = Guid.NewGuid();
        }
    }
}