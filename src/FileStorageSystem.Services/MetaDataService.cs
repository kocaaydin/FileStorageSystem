using AutoMapper;
using FileStorageSystem.Core.Commands;
using FileStorageSystem.Core.Dtos;
using FileStorageSystem.Core.Enums;
using FileStorageSystem.Core.Interfaces;
using FileStorageSystem.Data.Repositories;

namespace FileStorageSystem.Services;

public class MetaDataService(
        IMetaDataRepository metaDataRepository,
        IMapper mapper) : IMetaDataService
{
    public async Task AddChunkMetaDataAsync(ChunkMetaDataDto chunk)
    {
        await metaDataRepository.AddChunkMetaDataAsync(mapper.Map<CreateChunkMetaDataCommand>(chunk));
    }

    public async Task UpdateFileMetaDataStatusAsync(Guid fileId, FileMetaDataStatus fileMetaDataStatus)
    {
        await metaDataRepository.UpdateFileMetaDataStatusAsync(fileId, fileMetaDataStatus);
    }

    public async Task AddFileMetaDataAsync(FileMetaDataDto fileMetadata)
    {
        await metaDataRepository.AddFileMetaDataAsync(mapper.Map<CreateFileMetaDataCommand>(fileMetadata));
    }

    public async Task<FileMetaDataDto?> GetFileMetaDataWithChunksAsync(Guid fileId)
    {
        return mapper.Map<FileMetaDataDto>(await metaDataRepository.GetFileMetaDataWithChunksAsync(fileId));
    }
}