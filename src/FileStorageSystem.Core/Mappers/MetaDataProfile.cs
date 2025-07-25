using AutoMapper;
using FileStorageSystem.Core.Commands;
using FileStorageSystem.Core.Dtos;
using FileStorageSystem.Core.Models;

namespace FileStorageSystem.Core.Mappers;

public class MetaDataProfile: Profile
{
    public MetaDataProfile()
    {
        CreateMap<ChunkMetaData, ChunkMetaDataDto>().ReverseMap();
        CreateMap<FileMetaData, FileMetaDataDto>().ReverseMap();

        CreateMap<CreateChunkMetaDataCommand, ChunkMetaData>().ReverseMap();
        CreateMap<ChunkMetaDataDto, CreateChunkMetaDataCommand>();
        CreateMap<CreateFileMetaDataCommand, FileMetaData>();
        CreateMap<FileMetaDataDto, CreateFileMetaDataCommand>();
    }
}