using Microsoft.EntityFrameworkCore;
using SaasContentLibrary.Application.ContentBlocks.Queries;
using SaasContentLibrary.Domain.Common;
using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Infrastructure.Persistence.Queries
{
    public sealed class ContentBlockQueries(SaasContentLibraryDbContext dbContext)
        : IContentBlockQueries
    {
        public Task<ContentBlockResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var blockId = new ContentBlockId(id);

            return dbContext.ContentBlocks
                .AsNoTracking()
                .Where(cb => cb.Id == blockId)
                .Select(cb => new ContentBlockResponse
                (
                    cb.Id.Value,
                    cb.TenantId.Value,
                    cb.BlockType,
                    cb.Locale.Code,
                    cb.Name,
                    cb.Status,
                    cb.CreatedAtUtc,
                    cb.ArchivedAtUtc,
                    cb.Versions
                        .Where(v => v.VersionStatus == VersionStatus.Approved)
                        .Select(v => new ContentVersionResponse(
                            v.Id.Value,
                            v.VersionNumber,
                            v.ContentBody.Value,
                            v.VersionStatus,
                            v.AuthoredBy,
                            v.AuthoredOnUtc,
                            v.ApprovedBy,
                            v.ApprovedOnUtc))
                        .FirstOrDefault()))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ContentVersionResponse>?> GetVersionHistoryAsync(Guid contentBlockId, CancellationToken cancellationToken = default)
        {
            var blockId = new ContentBlockId(contentBlockId);

            var blockExists = await dbContext.ContentBlocks
                .AsNoTracking()
                .AnyAsync(cb => cb.Id == blockId, cancellationToken);

            if (!blockExists)
                return null;

            return await dbContext.ContentBlocks
                .AsNoTracking()
                .Where(cb => cb.Id == blockId)
                .SelectMany(cb => cb.Versions)
                .OrderByDescending(v => v.VersionNumber)
                .Select(v => new ContentVersionResponse(
                    v.Id.Value,
                    v.VersionNumber,
                    v.ContentBody.Value,
                    v.VersionStatus,
                    v.AuthoredBy,
                    v.AuthoredOnUtc,
                    v.ApprovedBy,
                    v.ApprovedOnUtc))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ContentBlockSummary>> ListByTypeAsync(Guid tenantId, BlockType? blockType, CancellationToken cancellationToken = default)
        {
            var tenant = new TenantId(tenantId);

            var query = dbContext.ContentBlocks
                .AsNoTracking()
                .Where(cb => cb.TenantId == tenant);

            if (blockType.HasValue)
            {
                query = query.Where(cb => cb.BlockType == blockType.Value);
            }

            return await query
                .OrderBy(cb => cb.Name)
                .Select(cb => new ContentBlockSummary
                (
                    cb.Id.Value,
                    cb.TenantId.Value,
                    cb.BlockType,
                    cb.Locale.Code,
                    cb.Name,
                    cb.Status,
                    cb.Versions
                        .Where(v => v.VersionStatus == VersionStatus.Approved)
                        .Select(v => (int?)v.VersionNumber)
                        .FirstOrDefault()
                ))
                .ToListAsync(cancellationToken);
        }
    }
}
