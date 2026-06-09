using MediatR;
using SaasContentLibrary.Domain.Common;
using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Application.ContentBlocks.Queries.ListBlocksByType;

public sealed record ListBlocksByTypeQuery(
    Guid TenantId,
    BlockType? BlockType)
    : IRequest<Result<IReadOnlyList<ContentBlockSummary>>>;
