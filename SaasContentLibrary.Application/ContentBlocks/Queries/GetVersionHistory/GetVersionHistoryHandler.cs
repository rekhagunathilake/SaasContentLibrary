using MediatR;
using SaasContentLibrary.Application.Common;
using SaasContentLibrary.Domain.Common;

namespace SaasContentLibrary.Application.ContentBlocks.Queries.GetVersionHistory
{
    public sealed class GetVersionHistoryHandler(IContentBlockQueries queries)
        : IRequestHandler<GetVersionHistoryQuery, Result<IReadOnlyList<ContentVersionResponse>>>
    {
        public async Task<Result<IReadOnlyList<ContentVersionResponse>>> Handle(GetVersionHistoryQuery request, CancellationToken cancellationToken)
        {
            var versions = await queries.GetVersionHistoryAsync(request.ContentBlockId, cancellationToken);

            return versions is null
                ? Result.Failure<IReadOnlyList<ContentVersionResponse>>(ApplicationErrors.ContentBlockNotFound(request.ContentBlockId))
                : Result.Success(versions);
        }
    }
}
