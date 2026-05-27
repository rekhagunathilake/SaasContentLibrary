using SaasContentLibrary.Domain.Common;

namespace SaasContentLibrary.Domain.ContentBlocks.Events
{
    public sealed record ContentBlockArchivedEvent(
    ContentBlockId ContentBlockId,
    DateTime OccurredOnUtc) : IDomainEvent;
}
