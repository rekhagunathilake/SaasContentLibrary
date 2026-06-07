using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using SaasContentLibrary.Api.Common;
using SaasContentLibrary.Api.RequestDTOs;
using SaasContentLibrary.Application.ContentBlocks.Commands.ApproveVersion;
using SaasContentLibrary.Application.ContentBlocks.Commands.Archive;
using SaasContentLibrary.Application.ContentBlocks.Commands.SubmitForReview;

namespace SaasContentLibrary.Api;

public static class ContentBlockEndpoints
{
    public static async Task<Results<Created<Guid>, ProblemHttpResult>> CreateContentBlock(
        CreateContentBlockRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(request.ToCommand(), cancellationToken);
        return result.IsSuccess
        ? TypedResults.Created($"/v1/content-blocks/{result.Value}", result.Value)
        : result.Error.ToProblemDetails();
    }

    public static async Task<Results<Created<Guid>, ProblemHttpResult>> AddDraftVersion(
        Guid id,
        AddDraftVersionRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(request.ToCommand(id), cancellationToken);
        return result.IsSuccess
            ? TypedResults.Created($"/v1/content-blocks/{id}/versions/{result.Value}", result.Value)
            : result.Error.ToProblemDetails();
    }

    public static async Task<Results<NoContent, ProblemHttpResult>> SubmitForReview(
        Guid id,
        Guid versionId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SubmitForReviewCommand(id, versionId), cancellationToken);
        return result.IsSuccess
            ? TypedResults.NoContent()
            : result.Error.ToProblemDetails();
    }

    public static async Task<Results<NoContent, ProblemHttpResult>> ApproveVersion(
        Guid id,
        Guid versionId,
        ApproveVersionRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ApproveVersionCommand(id, versionId, request.ApprovedBy), cancellationToken);
        return result.IsSuccess
            ? TypedResults.NoContent()
            : result.Error.ToProblemDetails();
    }

    public static async Task<Results<NoContent, ProblemHttpResult>> Archive(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ArchiveCommand(id), cancellationToken);
        return result.IsSuccess
            ? TypedResults.NoContent()
            : result.Error.ToProblemDetails();
    }
}
