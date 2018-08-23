namespace BlockchainSimulator.Node.BusinessLogic.Model.ValidationResults
{
    public struct ValidationResult
    {
        public bool IsSuccess { get; }
        public string[] Errors { get; }

        public ValidationResult(bool isSuccess, params string[] errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }
    }
}