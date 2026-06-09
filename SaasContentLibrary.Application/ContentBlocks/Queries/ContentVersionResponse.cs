using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Application.ContentBlocks.Queries;

public sealed record ContentVersionResponse(
    Guid Id,
    int VersionNumber,
    string Body,
    VersionStatus VersionStatus,
    string AuthoredBy,
    DateTime AuthoredAtUtc,
    string? ApprovedBy,
    DateTime? ApprovedAtUtc
    );
