using SaasContentLibrary.Domain.Common;
using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Domain.UnitTests.TestHelpers;

internal static class ContentBlockFactory
{
    public static readonly DateTime FixedNotUtc = new(2026, 6, 10, 12, 0, 0, DateTimeKind.Utc);

    public static ContentBlock CreateValid(
        string name = "Standard Disclaimer I",
        BlockType type = BlockType.Disclaimer,
        string locale = "en-US",
        string body = "This is confidential and intended for professional clients only.",
        string authoredBy = "rekha@test.com")
    {
        var localResult = Locale.Create(locale);
        var bodyResult = ContentBody.Create(body);

        return ContentBlock.Create(
            tenantId: TenantId.NewId(),
            blockType: type,
            locale: localResult.Value,
            name: name,
            initialBody: bodyResult.Value,
            authoredBy: authoredBy,
            nowUtc: FixedNotUtc).Value;
    }

    public static ContentBody TestBody(string value = "Test content text") => ContentBody.Create(value).Value;
}
