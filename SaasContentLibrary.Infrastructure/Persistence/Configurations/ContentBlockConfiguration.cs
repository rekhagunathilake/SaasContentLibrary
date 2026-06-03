using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaasContentLibrary.Domain.Common;
using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Infrastructure.Persistence.Configurations;

public sealed class ContentBlockConfiguration : IEntityTypeConfiguration<ContentBlock>
{
    public void Configure(EntityTypeBuilder<ContentBlock> builder)
    {
        builder.ToTable("content_blocks");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id)
            .HasConversion(id => id.Value, value => new ContentBlockId(value))
            .ValueGeneratedNever();

        builder.Property(b => b.TenantId)
            .HasConversion(id => id.Value, value => new TenantId(value));

        builder.Property(b => b.BlockType)
            .HasConversion<string>() // store enum names as strings
            .HasMaxLength(50);

        builder.Property(b => b.Locale)
            .HasConversion(
            locale => locale.Code,
            code => Locale.Create(code).Value)
            .HasMaxLength(20);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(ContentBlock.NameMaxLength);

        builder.Property(b => b.Status)
            .HasConversion<string>() // store enum names as strings
            .HasMaxLength(20);

        builder.Property(b => b.CreatedAtUtc);
        builder.Property(b => b.ArchivedAtUtc);

        builder.HasMany(b => b.Versions)
            .WithOne()
            .HasForeignKey("ContentBlockId") // shadow FK property
            .OnDelete(DeleteBehavior.Cascade);

        // EF needs to populate the private _version field when loading, not the read-only Versions property
        builder.Metadata.FindNavigation(nameof(ContentBlock.Versions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // Not a database-mapped property, so ignore it in EF configuration
        builder.Ignore(b => b.DomainEvents);

        // Index on TenantId for efficient querying by tenant
        builder.HasIndex(b => b.TenantId);
    }
}
