using SaasContentLibrary.Domain.Common;

namespace SaasContentLibrary.Domain.ContentBlocks.Events
{
    public sealed record ContentBlockCreatedEvent(
        ContentBlockId ContentBlockId,
        TenantId TenantId,
        BlockType BlockType,
        DateTime OccuredOnUtc) : IDomainEvent
    {
        public DateTime OccurredOnUtc => OccuredOnUtc;
    }
}
