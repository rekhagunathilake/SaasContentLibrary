using MediatR;
using SaasContentLibrary.Application.Common;
using SaasContentLibrary.Application.Common.Interfaces;
using SaasContentLibrary.Domain.Common;
using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Application.ContentBlocks.Commands.AddDraftVersion;

public sealed class AddDraftVersionHandler(
IContentBlockRepository repository,
IUnitOfWork unitOfWork,
TimeProvider timeProvider)
: IRequestHandler<AddDraftVersionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        AddDraftVersionCommand request,
        CancellationToken cancellationToken)
    {
        var block = await repository.GetByIdAsync(new ContentBlockId(request.ContentBlockId), cancellationToken);

        if (block == null)
        {
            return Result.Failure<Guid>(
                ApplicationErrors.ContentBlockNotFound(request.ContentBlockId));
        }

        var bodyResult = ContentBody.Create(request.ContentBody);
        if (bodyResult.IsFailure)
            return Result.Failure<Guid>(bodyResult.Error);

        var addResult = block.AddDraftVersion(bodyResult.Value, request.AuthoredBy, timeProvider.GetUtcNow().UtcDateTime);

        if (addResult.IsFailure)
            return Result.Failure<Guid>(addResult.Error);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(addResult.Value.Value);
    }
}
