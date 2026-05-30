using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Application.Common.Interfaces;

public interface IContentBlockRepository
{
    void Add(ContentBlock block);

    Task<ContentBlock?> GetByIdAsync(ContentBlockId id, CancellationToken ct = default);
}
