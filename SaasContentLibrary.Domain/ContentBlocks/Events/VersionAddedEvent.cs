using SaasContentLibrary.Domain.Common;

namespace SaasContentLibrary.Domain.ContentBlocks.Events
{
    public sealed record VersionAddedEvent(
    ContentBlockId ContentBlockId,
    ContentVersionId VersionId,
    int VersionNumber,
    DateTime OccurredOnUtc) : IDomainEvent;
}
