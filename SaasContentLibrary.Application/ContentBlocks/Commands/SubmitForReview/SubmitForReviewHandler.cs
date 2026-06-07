using MediatR;
using SaasContentLibrary.Application.Common;
using SaasContentLibrary.Application.Common.Interfaces;
using SaasContentLibrary.Domain.Common;
using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Application.ContentBlocks.Commands.SubmitForReview;

public sealed class SubmitForReviewHandler(IContentBlockRepository contentBlockRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IRequestHandler<SubmitForReviewCommand, Result>
{
    public async Task<Result> Handle(SubmitForReviewCommand request, CancellationToken cancellationToken)
    {
        var block = await contentBlockRepository.GetByIdAsync(new ContentBlockId(request.ContentBlockId), cancellationToken);

        if (block == null)
        {
            return Result.Failure(ApplicationErrors.ContentBlockNotFound(request.ContentBlockId));
        }

        var result = block.SubmitForReview(new ContentVersionId(request.VersionId), timeProvider.GetUtcNow().UtcDateTime);

        if (result.IsFailure)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
