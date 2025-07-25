using FileStorageSystem.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileStorageSystem.Data.Configurations;

public class ChunkMetadataConfiguration : BaseEntityTypeConfiguration<ChunkMetaData>
{
    public override void Configure(EntityTypeBuilder<ChunkMetaData> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.FileMetaDataId).IsRequired();

        builder.Property(c => c.ChunkIndex).IsRequired();

        builder.Property(c => c.Size).IsRequired();

        builder.Property(c => c.StorageProviderType).IsRequired();

        builder.Property(c => c.Checksum).IsRequired()
                                         .HasMaxLength(64);
    }
}
