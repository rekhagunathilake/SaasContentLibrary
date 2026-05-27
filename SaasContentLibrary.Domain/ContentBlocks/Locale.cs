using SaasContentLibrary.Domain.Common;
using System.Text.RegularExpressions;

namespace SaasContentLibrary.Domain.ContentBlocks
{
    public sealed partial record Locale
    {
        public string Code { get; }

        private Locale(string code) => Code = code;

        public static Result<Locale> Create(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Result.Failure<Locale>(ContentBlockErrors.LocaleEmpty);

            if (!LocaleRegex().IsMatch(code))
                return Result.Failure<Locale>(ContentBlockErrors.LocaleInvalid(code));

            return Result.Success(new Locale(code));
        }

        public override string ToString() => Code;

        // Permissive BCP-47: en, en-US, zh-Hans, zh-Hans-CN
        [GeneratedRegex(@"^[a-z]{2,3}(-[A-Z][a-z]{3})?(-[A-Z]{2})?$")]
        private static partial Regex LocaleRegex();

    }
}
