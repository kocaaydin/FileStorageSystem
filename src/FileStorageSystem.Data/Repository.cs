using System.Linq.Expressions;
using FileStorageSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace FileStorageSystem.Data;

public class Repository<TDbContext>(TDbContext dbContext) : IRepository<TDbContext> where TDbContext : DbContext
{
    public async Task AddAsync<T>(T entity, bool saveChanges = true) where T : BaseModel
    {
        await dbContext.Set<T>().AddAsync(entity);

        await SaveChangesAsync(saveChanges);
    }

    public async Task DeleteAsync<T>(Guid id, bool saveChanges = true) where T : BaseModel
    {
        var entity = dbContext.Set<T>().Find(id);
        
        if (entity != null)
        {
            dbContext.Set<T>().Remove(entity);
        }

        await SaveChangesAsync(saveChanges);
    }

    public Task<T?> GetByIdAsync<T>(Guid id, bool asNoTracking = true) where T : BaseModel
    {
        IQueryable<T> query = dbContext.Set<T>();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public IQueryable<T> GetQueryable<T>(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, bool asNoTracking = true) where T : class
    {
        IQueryable<T> query = dbContext.Set<T>();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (include != null)
        {
            query = include(query);
        }

        return query;
    }

    public Task UpdateAsync<T>(T entity, bool saveChanges = true) where T : BaseModel
    {
        dbContext.Set<T>().Update(entity);
        return SaveChangesAsync(true);
    }

    public async Task SaveChangesAsync()
    {
        await SaveChangesAsync(true);
    }
    private async Task SaveChangesAsync(bool saveChanges = true)
    {
        if (saveChanges)
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
