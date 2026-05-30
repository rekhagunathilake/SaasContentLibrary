using MediatR;
using SaasContentLibrary.Application.Common.Interfaces;
using SaasContentLibrary.Domain.Common;
using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Application.ContentBlocks.Commands.CreateContentBlock;

public sealed class CreateContentBlockHandler(
    IContentBlockRepository repository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IRequestHandler<CreateContentBlockCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateContentBlockCommand request,
        CancellationToken cancellationToken)
    {
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        var localeResult = Locale.Create(request.LocaleCode);
        if (localeResult.IsFailure)
            return Result.Failure<Guid>(localeResult.Error);

        var bodyResult = ContentBody.Create(request.ContentBody);
        if (bodyResult.IsFailure)
            return Result.Failure<Guid>(bodyResult.Error);

        var blockResult = ContentBlock.Create(
            new TenantId(request.TenantId),
            request.BlockType,
            localeResult.Value,
            request.Name,
            bodyResult.Value,
            request.AuthoredBy,
            nowUtc);
        if (blockResult.IsFailure)
            return Result.Failure<Guid>(blockResult.Error);

        repository.Add(blockResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(blockResult.Value.Id.Value);
    }
}
