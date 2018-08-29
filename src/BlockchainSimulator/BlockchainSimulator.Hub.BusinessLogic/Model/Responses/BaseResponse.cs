using System;

namespace BlockchainSimulator.Hub.BusinessLogic.Model.Responses
{
    public abstract class BaseResponse<T>
    {
        public Guid Id { get; }
        public abstract bool IsSuccess { get; }
        public string Message { get; }
        public T Result { get; }

        protected BaseResponse(string message, T result)
        {
            Message = message;
            Result = result;
            Id = Guid.NewGuid();
        }
    }
}