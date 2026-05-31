using SaasContentLibrary.Application.Common.Interfaces;
using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Infrastructure.Persistence
{
    public sealed class UnitOfWork(SaasContentLibraryDbContext dbContext) : IUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            var result = await dbContext.SaveChangesAsync(ct);

            var tracked = dbContext.ChangeTracker.Entries<ContentBlock>()
                .Select(e => e.Entity);

            foreach (var block in tracked)
                block.ClearDomainEvents();

            return result;
        }
    }
}
