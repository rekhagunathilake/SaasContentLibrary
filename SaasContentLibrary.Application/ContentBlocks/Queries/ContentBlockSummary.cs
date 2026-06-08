using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Application.ContentBlocks.Queries;

public sealed record ContentBlockSummary(
    Guid Id,
    Guid TenantId,
    BlockType BlockType,
    string LocaleCode,
    string Name,
    BlockStatus BlockStatus,
    int? CurrentApprovedVersionNumber
);
