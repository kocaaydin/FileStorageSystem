using AutoMapper;
using FileStorageSystem.Core.Dtos;
using FileStorageSystem.Core.Models;

namespace FileStorageSystem.Core.Mappers;

public class MetaDataProfile: Profile
{
    public MetaDataProfile()
    {
        CreateMap<ChunkMetaData, ChunkMetaDataWithProviderDto>().ReverseMap();
        CreateMap<FileMetaDataDto, FileMetaData>().ReverseMap();
        CreateMap<ChunkMetaDataDto, ChunkMetaDataWithProviderDto>().ReverseMap();
    }
}