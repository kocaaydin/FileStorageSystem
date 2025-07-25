using System.Linq.Expressions;
using FileStorageSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace FileStorageSystem.Data;

public interface IRepository<TDbContext> where TDbContext : DbContext   
{
    IQueryable<T> GetQueryable<T>(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, bool asNoTracking = true) where T : class;

    Task<T?> GetByIdAsync<T>(Guid id, bool asNoTracking = true) where T : BaseModel;

    Task AddAsync<T>(T entity, bool saveChanges = true) where T : BaseModel;

    Task UpdateAsync<T>(T entity, bool saveChanges = true) where T : BaseModel;

    Task DeleteAsync<T>(Guid id, bool saveChanges = true) where T : BaseModel;
    
    Task SaveChangesAsync();
}
