using FileStorageSystem.Core.Commands;
using FileStorageSystem.Core.Enums;
using FileStorageSystem.Core.Models;

namespace FileStorageSystem.Data.Repositories;

public interface IMetaDataRepository
{
    Task AddChunkMetaDataAsync(CreateChunkMetaDataCommand command);
    Task AddFileMetaDataAsync(CreateFileMetaDataCommand fileMetadata);
    Task<FileMetaData?> GetFileMetaDataWithChunksAsync(Guid fileId);
    Task UpdateFileMetaDataStatusAsync(Guid fileId, FileMetaDataStatus fileMetaDataStatus);
}
