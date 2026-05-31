using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Infrastructure.Persistence.Configurations;

public sealed class ContentVersionConfiguration : IEntityTypeConfiguration<ContentVersion>
{
    public void Configure(EntityTypeBuilder<ContentVersion> builder)
    {
        builder.ToTable("content_versions");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id)
            .HasConversion(id => id.Value, value => new ContentVersionId(value))
            .ValueGeneratedNever();

        builder.Property(v => v.VersionNumber);

        builder.Property(v => v.ContentBody)
            .HasConversion(
               contentBody => contentBody.Value,
                value => ContentBody.Create(value).Value)
            .IsRequired()
            .HasMaxLength(ContentBody.MaxLength);

        builder.Property(v => v.VersionStatus)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(v => v.AuthoredBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.AuthoredOnUtc);
        builder.Property(v => v.ApprovedBy)
            .HasMaxLength(200);
        builder.Property(v => v.ApprovedOnUtc);

        builder.HasIndex("ContentBlockId", nameof(ContentVersion.VersionNumber))
            .IsUnique();
    }
}