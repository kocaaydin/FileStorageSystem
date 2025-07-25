using FileStorageSystem.Core.Enums;

namespace FileStorageSystem.Core.Interfaces;

public interface IStorageProvider
{
    StorageProviderType ProviderType { get; }
    Task SaveChunkAsync(Guid chunkId, Stream chunkData, string originalFileName);
    Task<Stream> GetChunkAsync(Guid chunkId, string originalFileName);
}
