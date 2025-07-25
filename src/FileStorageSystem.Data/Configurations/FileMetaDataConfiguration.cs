using FileStorageSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileStorageSystem.Data.Configurations;

public class FileMetaDataConfiguration: BaseEntityTypeConfiguration<FileMetaData>
{
    public override void Configure(EntityTypeBuilder<FileMetaData> builder)
    {
        base.Configure(builder);

        builder.HasMany(fm => fm.ChunkMetaDatas)
               .WithOne()
               .HasForeignKey(c => c.FileMetaDataId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(fm => fm.FileName).IsRequired()
                                            .HasMaxLength(256);

        builder.Property(fm => fm.OriginalSize).IsRequired();

        builder.Property(fm => fm.OriginalChecksum).IsRequired()
                                                   .HasMaxLength(64);

        builder.Property(fm => fm.UploadDate).IsRequired();

        builder.Property(fm => fm.FileMetaDataStatus).IsRequired();
    }
}
