using AutoMapper;
using FileStorageSystem.Core.Commands;
using FileStorageSystem.Core.Dtos;
using FileStorageSystem.Core.Enums;
using FileStorageSystem.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorageSystem.Data.Repositories;

public class MetaDataRepository(IRepository<AppDbContext> genericRepository, IMapper mapper) : IMetaDataRepository
{
    public async Task<FileMetaData?> GetFileMetaDataWithChunksAsync(Guid fileId)
    {
        return await genericRepository.GetQueryable<FileMetaData>(
            filter: fm => fm.Id == fileId,
            include: query => query.Include(fm => fm.ChunkMetaDatas),
            asNoTracking: true
        ).FirstOrDefaultAsync();
    }

    public async Task<List<ChunkMetaData>> GetChunksForFileSortedByIndexAsync(Guid fileId)
    {
        return await genericRepository.GetQueryable<ChunkMetaData>(
            filter: c => c.FileMetaDataId == fileId,
            asNoTracking: true
        ).OrderBy(c => c.ChunkIndex).ToListAsync();
    }

    public async Task AddChunkMetaDataAsync(CreateChunkMetaDataCommand chunk)
    {
        await genericRepository.AddAsync(mapper.Map<ChunkMetaData>(chunk));
        await genericRepository.SaveChangesAsync();
    }

    public async Task UpdateFileMetaDataStatusAsync(Guid fileId, FileMetaDataStatus fileMetaDataStatus)
    {
        var fileMetadata = await genericRepository.GetByIdAsync<FileMetaData>(fileId, asNoTracking: false);
        if (fileMetadata != null)
        {
            fileMetadata.FileMetaDataStatus = fileMetaDataStatus;
            await genericRepository.UpdateAsync(fileMetadata);
            await genericRepository.SaveChangesAsync();
        }
    }

    public async Task AddFileMetaDataAsync(CreateFileMetaDataCommand fileMetadata)
    {
        await genericRepository.AddAsync(mapper.Map<FileMetaData>(fileMetadata));
        await genericRepository.SaveChangesAsync();
    }
}
