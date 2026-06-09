using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Application.ContentBlocks.Queries;

public sealed record ContentBlockResponse(
    Guid Id,
    Guid TenantId,
    BlockType BlockType,
    string LocaleCode,
    string Name,
    BlockStatus BlockStatus,
    DateTime CreatedAtUtc,
    DateTime? ArchivedAtUtc,
    ContentVersionResponse? CurrentApprovedVersion);
