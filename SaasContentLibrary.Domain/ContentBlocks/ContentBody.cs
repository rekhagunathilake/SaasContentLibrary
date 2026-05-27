using SaasContentLibrary.Domain.Common;

namespace SaasContentLibrary.Domain.ContentBlocks
{
    public sealed record ContentBody
    {
        public const int MaxLength = 10000;

        public string Value { get; }

        private ContentBody(string value) => Value = value;

        public static Result<ContentBody> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Failure<ContentBody>(ContentBlockErrors.BodyEmpty);

            if (value.Length > MaxLength)
                return Result.Failure<ContentBody>(ContentBlockErrors.BodyTooLong(value.Length));

            return Result.Success(new ContentBody(value));
        }

        public override string ToString() => Value;
    }
}
