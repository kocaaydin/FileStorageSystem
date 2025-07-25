using FileStorageSystem.Core.Enums;

namespace FileStorageSystem.Core.Constants;

public class ProviderDefinitions
{
    public static readonly List<StorageProviderType>? AvailableProviders = [.. Enum.GetValues(typeof(StorageProviderType)).Cast<StorageProviderType>()];
}
