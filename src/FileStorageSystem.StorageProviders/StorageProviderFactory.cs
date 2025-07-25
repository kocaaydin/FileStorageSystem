using FileStorageSystem.Core.Enums;
using FileStorageSystem.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FileStorageSystem.StorageProviders;

public class StorageProviderFactory(IServiceProvider serviceProvider)
{
    public IStorageProvider GetProvider(StorageProviderType providerType)
    {
        return providerType switch
        {
            StorageProviderType.FileSystem => serviceProvider.GetRequiredService<FileSystemStorageProvider>(),
      
            _ => throw new ArgumentOutOfRangeException(nameof(providerType), $"No storage provider found for type {providerType}")
        };
    }
}
