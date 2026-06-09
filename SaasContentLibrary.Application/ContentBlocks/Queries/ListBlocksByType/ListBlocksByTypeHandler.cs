using MediatR;
using SaasContentLibrary.Domain.Common;

namespace SaasContentLibrary.Application.ContentBlocks.Queries.ListBlocksByType;

public sealed class ListBlocksByTypeHandler(IContentBlockQueries queries)
    : IRequestHandler<ListBlocksByTypeQuery, Result<IReadOnlyList<ContentBlockSummary>>>
{
    public async Task<Result<IReadOnlyList<ContentBlockSummary>>> Handle(ListBlocksByTypeQuery request, CancellationToken cancellationToken)
    {
        var results = await queries.ListByTypeAsync(
            request.TenantId, request.BlockType, cancellationToken);

        return Result.Success(results);
    }
}
