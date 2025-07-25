using AutoMapper;
using FileStorageSystem.Core.Commands;
using FileStorageSystem.Core.Dtos;
using FileStorageSystem.Core.Enums;
using FileStorageSystem.Core.Interfaces;
using FileStorageSystem.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace FileStorageSystem.Services;

public class MetaDataService(
        IMetaDataRepository metaDataRepository,
        IMapper mapper,
        ILogger<MetaDataService> logger) : IMetaDataService
{
    public async Task AddChunkMetaDataAsync(ChunkMetaDataDto chunk)
    {
        logger.LogInformation("Chunk meta verisi ekleniyor: ChunkId={ChunkId}, FileMetaDataId={FileMetaDataId}",
            chunk.Id, chunk.FileMetaDataId);

        await metaDataRepository.AddChunkMetaDataAsync(mapper.Map<CreateChunkMetaDataCommand>(chunk));
    }

    public async Task UpdateFileMetaDataStatusAsync(Guid fileId, FileMetaDataStatus fileMetaDataStatus)
    {
        logger.LogInformation("Dosya durumu güncelleniyor: FileId={FileId}, Status={Status}", fileId, fileMetaDataStatus);

        await metaDataRepository.UpdateFileMetaDataStatusAsync(fileId, fileMetaDataStatus);
    }

    public async Task AddFileMetaDataAsync(FileMetaDataDto fileMetadata)
    {
        logger.LogInformation("Dosya meta verisi ekleniyor: FileId={FileId}, FileName={FileName}",
            fileMetadata.Id, fileMetadata.FileName);

        await metaDataRepository.AddFileMetaDataAsync(mapper.Map<CreateFileMetaDataCommand>(fileMetadata));
    }

    public async Task<FileMetaDataDto?> GetFileMetaDataWithChunksAsync(Guid fileId)
    {
        logger.LogInformation("Dosya meta verisi getiriliyor: FileId={FileId}", fileId);
        
        return mapper.Map<FileMetaDataDto>(await metaDataRepository.GetFileMetaDataWithChunksAsync(fileId));
    }
}