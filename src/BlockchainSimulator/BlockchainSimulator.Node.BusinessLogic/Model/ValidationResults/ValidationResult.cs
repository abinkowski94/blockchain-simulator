namespace BlockchainSimulator.Node.BusinessLogic.Model.ValidationResults
{
    public struct ValidationResult
    {
        public string[] Errors { get; }
        public bool IsSuccess { get; }

        public ValidationResult(bool isSuccess, params string[] errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }
    }
}