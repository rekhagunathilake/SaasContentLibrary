using MediatR;
using SaasContentLibrary.Application.Common;
using SaasContentLibrary.Application.Common.Interfaces;
using SaasContentLibrary.Domain.Common;
using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Application.ContentBlocks.Commands.Archive;

public sealed class ArchiveHandler(IContentBlockRepository contentBlockRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<ArchiveCommand, Result>
{
    public async Task<Result> Handle(ArchiveCommand request, CancellationToken cancellationToken)
    {
        var block = await contentBlockRepository.GetByIdAsync(new ContentBlockId(request.ContentBlockId), cancellationToken);

        if (block == null)
        {
            return Result.Failure(ApplicationErrors.ContentBlockNotFound(request.ContentBlockId));
        }

        var result = block.Archive(timeProvider.GetUtcNow().UtcDateTime);

        if (result.IsFailure)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
