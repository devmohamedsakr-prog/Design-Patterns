namespace OrderValidation.After.Models
{
    /// <summary>
    /// ValidationResult: Result object from validation chain
    /// SRP: Only encapsulates validation result
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public string HandlerName { get; set; }

        public ValidationResult(bool isValid, string errorMessage = "", string handlerName = "")
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
            HandlerName = handlerName;
        }

        public override string ToString() =>
            IsValid ? "✓ Validation passed" : $"✗ Validation failed: {ErrorMessage}";
    }
}
