using FileStorageSystem.Core.Dtos;
using FileStorageSystem.Core.Enums;
using FileStorageSystem.Core.Models;

namespace FileStorageSystem.Data.Repositories;

public interface IMetaDataRepository
{
    Task AddChunkMetaDataAsync(ChunkMetaDataDto chunk);
    Task AddFileMetaDataAsync(FileMetaDataDto fileMetadata);
    Task<FileMetaData?> GetFileMetaDataWithChunksAsync(Guid fileId);
    Task UpdateFileMetaDataStatusAsync(Guid fileId, FileMetaDataStatus fileMetaDataStatus);
}
