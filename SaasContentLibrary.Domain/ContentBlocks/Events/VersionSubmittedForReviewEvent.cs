using SaasContentLibrary.Domain.Common;

namespace SaasContentLibrary.Domain.ContentBlocks.Events
{
    public sealed record VersionSubmittedForReviewEvent(
    ContentBlockId ContentBlockId,
    ContentVersionId VersionId,
    DateTime OccurredOnUtc) : IDomainEvent;
}
