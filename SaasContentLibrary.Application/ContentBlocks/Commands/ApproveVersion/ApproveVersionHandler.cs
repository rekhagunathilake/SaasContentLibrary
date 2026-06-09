using MediatR;
using SaasContentLibrary.Application.Common;
using SaasContentLibrary.Application.Common.Interfaces;
using SaasContentLibrary.Domain.Common;
using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Application.ContentBlocks.Commands.ApproveVersion;

public sealed class ApproveVersionHandler(
    IContentBlockRepository contentBlockRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<ApproveVersionCommand, Result>
{
    public async Task<Result> Handle(ApproveVersionCommand request, CancellationToken cancellationToken)
    {
        var block = await contentBlockRepository.GetByIdAsync(new ContentBlockId(request.ContentBlockId), cancellationToken);

        if (block == null)
            return Result.Failure(ApplicationErrors.ContentBlockNotFound(request.ContentBlockId));

        var result = block.ApproveVersion(new ContentVersionId(request.VersionId), request.ApprovedBy, timeProvider.GetUtcNow().UtcDateTime);

        if (result.IsFailure)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
