using SaasContentLibrary.Domain.Common;

namespace SaasContentLibrary.Domain.ContentBlocks
{
    public static class ContentBlockErrors
    {
        public static readonly Error NameEmpty =
        Error.Validation("ContentBlock.NameEmpty", "Name cannot be empty.");

        public static Error NameTooLong(int actual) =>
            Error.Validation("ContentBlock.NameTooLong",
                $"Name exceeds maximum length of {ContentBlock.NameMaxLength} (was {actual}).");

        public static readonly Error LocaleEmpty =
            Error.Validation("Locale.Empty", "Locale code cannot be empty.");

        public static Error LocaleInvalid(string code) =>
            Error.Validation("Locale.Invalid", $"Locale code '{code}' is not a valid BCP-47 code.");

        public static readonly Error BodyEmpty =
            Error.Validation("ContentBody.Empty", "Body cannot be empty.");

        public static Error BodyTooLong(int actual) =>
            Error.Validation("ContentBody.TooLong",
                $"Body exceeds maximum length of {ContentBody.MaxLength} (was {actual}).");

        public static readonly Error AuthorEmpty =
            Error.Validation("ContentVersion.AuthorEmpty", "Authored-by cannot be empty.");

        public static readonly Error ApproverEmpty =
            Error.Validation("ContentVersion.ApproverEmpty", "Approved-by cannot be empty.");

        public static readonly Error IsArchived =
            Error.Conflict("ContentBlock.IsArchived", "Cannot modify an archived content block.");

        public static readonly Error VersionNotFound =
            Error.NotFound("ContentVersion.NotFound", "Version not found in this content block.");

        public static readonly Error VersionNotDraft =
            Error.Conflict("ContentVersion.NotDraft",
                "Only a version in Draft status can be submitted for review.");

        public static readonly Error VersionNotInReview =
            Error.Conflict("ContentVersion.NotInReview",
                "Only a version in InReview status can be approved.");
    }
}
