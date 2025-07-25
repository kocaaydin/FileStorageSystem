using FileStorageSystem.Core.Dtos;
using FileStorageSystem.Core.Enums;

namespace FileStorageSystem.Core.Interfaces;

public interface IMetaDataService
{
    Task AddChunkMetaDataAsync(ChunkMetaDataDto chunk);

    Task UpdateFileMetaDataStatusAsync(Guid fileId, FileMetaDataStatus fileMetaDataStatus);

    Task<FileMetaDataDto?> GetFileMetaDataWithChunksAsync(Guid fileId);

    Task AddFileMetaDataAsync(FileMetaDataDto fileMetadata);
}
