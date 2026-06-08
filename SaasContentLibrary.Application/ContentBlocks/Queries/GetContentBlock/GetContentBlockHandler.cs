using MediatR;
using SaasContentLibrary.Application.Common;
using SaasContentLibrary.Domain.Common;

namespace SaasContentLibrary.Application.ContentBlocks.Queries.GetContentBlock;

public sealed class GetContentBlockHandler(IContentBlockQueries queries)
    : IRequestHandler<GetContentBlockQuery, Result<ContentBlockResponse>>
{
    public async Task<Result<ContentBlockResponse>> Handle(GetContentBlockQuery request, CancellationToken cancellationToken)
    {
        var response = await queries.GetByIdAsync(request.Id, cancellationToken);

        return response is null
            ? Result.Failure<ContentBlockResponse>(ApplicationErrors.ContentBlockNotFound(request.Id))
            : Result.Success(response);
    }
}
