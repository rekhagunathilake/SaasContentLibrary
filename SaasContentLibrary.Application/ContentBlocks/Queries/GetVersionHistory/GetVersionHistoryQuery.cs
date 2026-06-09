using MediatR;
using SaasContentLibrary.Domain.Common;

namespace SaasContentLibrary.Application.ContentBlocks.Queries.GetVersionHistory;

public sealed record GetVersionHistoryQuery(Guid ContentBlockId)
    : IRequest<Result<IReadOnlyList<ContentVersionResponse>>>;
