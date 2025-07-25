using FileStorageSystem.Core.Dtos;
using FileStorageSystem.Core.Enums;
using FileStorageSystem.Core.Helpers;
using FileStorageSystem.Core.Interfaces;
using FileStorageSystem.StorageProviders;

namespace FileStorageSystem.Services;


//Core tarafına utilize taşınabilrir
internal class ChunkerService(IChecksumCalculator checksumCalculator,
                            IChunkSizeCalculator chunkSizeCalculator,
                            StorageProviderFactory storageProviderFactory) : IChunkerService
{
    public async IAsyncEnumerable<ChunkDataDto> GetChunksAsync(string filePath)
    {
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var buffer = new byte[chunkSizeCalculator.CalculateOptimalChunkSize(fileStream.Length)];
        int bytesRead;
        int chunkIndex = 0;

        while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            var chunkStream = new MemoryStream(buffer, 0, bytesRead, writable: false, publiclyVisible: true);

            var chunkChecksum = await checksumCalculator.CalculateSha256Async(chunkStream);

            chunkStream.Seek(0, SeekOrigin.Begin);

            yield return new ChunkDataDto
            {
                ChunkMetaData = new()
                {
                    Id = Guid.NewGuid(),
                    ChunkIndex = chunkIndex,
                    Size = bytesRead,
                    Checksum = chunkChecksum
                },
                Data = chunkStream
            };

            chunkIndex++;
        }
    }

    public async Task<Stream> MergeFileAsync(FileMetaDataDto fileMetaData)
    {
        var sortedChunks = fileMetaData.Chunks.OrderBy(c => c.ChunkIndex).ToList();
        var memoryStream = new MemoryStream();

        foreach (var chunk in sortedChunks)
        {
            var providerType = (StorageProviderType)Enum.Parse(typeof(StorageProviderType), chunk.StorageProviderType.ToString());
            var storageProvider = storageProviderFactory.GetProvider(providerType);
            using var chunkStream = await storageProvider.GetChunkAsync(chunk.Id, fileMetaData.FileName);

            await chunkStream.CopyToAsync(memoryStream);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }
}
