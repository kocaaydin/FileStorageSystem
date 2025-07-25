using FileStorageSystem.Core.Constants;
using FileStorageSystem.Core.Enums;
using FileStorageSystem.Core.Interfaces;

namespace FileStorageSystem.Services;

public class ProviderService : IProviderService
{
    public List<StorageProviderType> GetAvailableProviders()
    {
        return ProviderDefinitions.AvailableProviders!;
    }
}
