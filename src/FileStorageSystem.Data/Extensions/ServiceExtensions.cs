using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FileStorageSystem.Data.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddEfDbContext<TDbContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction, ServiceLifetime lifetime = ServiceLifetime.Scoped, bool convertEmptyStringsToNull = false) where TDbContext : DbContext
    {
        services.AddDbContext<TDbContext>(optionsAction, contextLifetime: lifetime);

        services.Add(new ServiceDescriptor(
           typeof(IRepository<TDbContext>),
           serviceProvider =>
           {
               TDbContext dbContext = ActivatorUtilities.CreateInstance<TDbContext>(serviceProvider);
               return new Repository<TDbContext>(dbContext);
           },
           lifetime));

        return services;
    } 
}