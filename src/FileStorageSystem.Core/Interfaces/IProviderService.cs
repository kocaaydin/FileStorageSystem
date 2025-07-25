using System;
using FileStorageSystem.Core.Enums;

namespace FileStorageSystem.Core.Interfaces;

public interface IProviderService
{
    List<StorageProviderType> GetAvailableProviders();
}
