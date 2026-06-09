using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using SaasContentLibrary.Api.Common;
using SaasContentLibrary.Api.RequestDTOs;
using SaasContentLibrary.Application.ContentBlocks.Commands.ApproveVersion;
using SaasContentLibrary.Application.ContentBlocks.Commands.Archive;
using SaasContentLibrary.Application.ContentBlocks.Commands.SubmitForReview;
using SaasContentLibrary.Application.ContentBlocks.Queries;
using SaasContentLibrary.Application.ContentBlocks.Queries.GetContentBlock;
using SaasContentLibrary.Application.ContentBlocks.Queries.GetVersionHistory;
using SaasContentLibrary.Application.ContentBlocks.Queries.ListBlocksByType;

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

    public static async Task<Results<Ok<ContentBlockResponse>, ProblemHttpResult>> GetContentBlock(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetContentBlockQuery(id), cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.ToProblemDetails();
    }

    public static async Task<Ok<IReadOnlyList<ContentBlockSummary>>> ListContentBlocks(
        [AsParameters] ListContentBlocksRequest filter,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ListBlocksByTypeQuery(filter.TenantId, filter.BlockType), cancellationToken);

        return TypedResults.Ok(result.Value);
    }

    public static async Task<Results<Ok<IReadOnlyList<ContentVersionResponse>>, ProblemHttpResult>> GetVersionHistory(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetVersionHistoryQuery(id), cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.ToProblemDetails();
    }
}
