namespace SaasContentLibrary.Domain.Common
{
    public sealed record Error(string code, string message, ErrorType type)
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);

        public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);
        public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);
        public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);
    }
}
