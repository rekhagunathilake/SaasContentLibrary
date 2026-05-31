using Microsoft.EntityFrameworkCore;
using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Infrastructure.Persistence;

public sealed class SaasContentLibraryDbContext(DbContextOptions<SaasContentLibraryDbContext> options) : DbContext(options)
{
    public DbSet<ContentBlock> ContentBlocks => Set<ContentBlock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("content");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SaasContentLibraryDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
