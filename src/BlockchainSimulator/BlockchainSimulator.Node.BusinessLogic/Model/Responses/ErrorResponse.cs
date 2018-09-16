using System;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Responses
{
    public class ErrorResponse<T> : BaseResponse<T>
    {
        public string[] Errors { get; }
        public override bool IsSuccess => false;

        public ErrorResponse(string message, T result, params string[] errors) : base(message, result)
        {
            if (!message.StartsWith("The encoded block has been already"))
            {
                Console.WriteLine(message);
            }

            Errors = errors;
        }
    }
}