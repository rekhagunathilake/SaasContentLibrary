using SaasContentLibrary.Domain.Common;

namespace SaasContentLibrary.Domain.ContentBlocks.Events
{
    public sealed record VersionApprovedEvent(
    ContentBlockId ContentBlockId,
    ContentVersionId ApprovedVersionId,
    ContentVersionId? SupersededVersionId,
    DateTime OccurredOnUtc) : IDomainEvent;
}
