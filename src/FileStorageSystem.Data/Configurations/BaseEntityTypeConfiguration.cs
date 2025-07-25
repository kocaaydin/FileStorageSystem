using FileStorageSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileStorageSystem.Data.Configurations;

public class BaseEntityTypeConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseModel
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.CreateDate).IsRequired();
    }
}
