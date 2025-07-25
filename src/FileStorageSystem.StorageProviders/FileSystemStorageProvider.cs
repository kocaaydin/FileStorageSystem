using FileStorageSystem.Core;
using FileStorageSystem.Core.Enums;
using FileStorageSystem.Core.Exceptions;
using FileStorageSystem.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace FileStorageSystem.StorageProviders;

public class FileSystemStorageProvider(IOptions<StorageSettings> storageSettings) : IStorageProvider
{
    public StorageProviderType ProviderType => StorageProviderType.FileSystem;

    public async Task SaveChunkAsync(Guid chunkId, Stream chunkData, string originalFileName)
    {
        var filePath = Path.Combine(storageSettings.Value.FileSystemStoragePath, $"{originalFileName}_{chunkId}.chunk");

        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            chunkData.Seek(0, SeekOrigin.Begin);
            await chunkData.CopyToAsync(fileStream);
        }
    }

    public async Task<Stream> GetChunkAsync(Guid chunkId, string originalFileName)
    {
        var filePath = Path.Combine(storageSettings.Value.FileSystemStoragePath, $"{originalFileName}_{chunkId}.chunk");

        if (!File.Exists(filePath))
        {
            throw new ChunkNotFoundException($"Failed to retrieve chunk: {chunkId}");
        }

        var memoryStream = new MemoryStream();
        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            await fileStream.CopyToAsync(memoryStream);
        }
        memoryStream.Seek(0, SeekOrigin.Begin);

        return memoryStream;
    }
}
