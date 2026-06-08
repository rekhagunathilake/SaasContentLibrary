using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Application.ContentBlocks.Queries;

public interface IContentBlockQueries
{
    Task<ContentBlockResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ContentBlockSummary>> ListByTypeAsync(Guid tenantId, BlockType? blockType, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ContentVersionResponse>?> GetVersionHistoryAsync(Guid contentBlockId, CancellationToken cancellationToken = default);
}
