using Microsoft.EntityFrameworkCore;
using SaasContentLibrary.Application.Common.Interfaces;
using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Infrastructure.Persistence.Repositories
{
    public sealed class ContentBlockRepository(SaasContentLibraryDbContext dbContext) : IContentBlockRepository
    {
        public void Add(ContentBlock block) => dbContext.ContentBlocks.Add(block);

        public Task<ContentBlock?> GetByIdAsync(ContentBlockId id, CancellationToken ct = default) =>
            dbContext.ContentBlocks
            .Include(cb => cb.Versions)
            .FirstOrDefaultAsync(cb => cb.Id == id, ct);
    }
}
