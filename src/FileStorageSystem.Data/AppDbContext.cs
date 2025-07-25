using Microsoft.EntityFrameworkCore;

namespace FileStorageSystem.Data; 

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("public");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var updatedEntries = ChangeTracker.Entries().Where(x => x.State == EntityState.Modified).ToList();

        updatedEntries.ForEach(u =>
        {
            u.Property("UpdateDate").CurrentValue = DateTime.UtcNow;
        });

        return base.SaveChangesAsync(cancellationToken);
    }
}
