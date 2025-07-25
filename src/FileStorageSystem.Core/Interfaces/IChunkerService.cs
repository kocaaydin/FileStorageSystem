using FileStorageSystem.Core.Dtos;

namespace FileStorageSystem.Core.Interfaces;

public interface IChunkerService
{
    IAsyncEnumerable<ChunkDataDto> GetChunksAsync(string filePath);
    Task<Stream> MergeFileAsync(FileMetaDataDto fileMetaData);
}
