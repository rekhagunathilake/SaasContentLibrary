using MediatR;
using SaasContentLibrary.Domain.Common;

namespace SaasContentLibrary.Application.ContentBlocks.Commands.SubmitForReview;

public sealed record SubmitForReviewCommand(
    Guid ContentBlockId,
    Guid VersionId) : IRequest<Result>;