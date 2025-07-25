using FileStorageSystem.Core.Enums;

namespace FileStorageSystem.Core.Dtos;

public class ChunkMetaDataWithProviderDto : BaseDto
{
    public ChunkMetaDataWithProviderDto()
    {

    }
    
    public ChunkMetaDataWithProviderDto(string checkSum, StorageProviderType storageProviderType)
    {
        Checksum = checkSum;
        StorageProviderType = storageProviderType;
    }
    public Guid FileMetaDataId { get; set; }
    public int ChunkIndex { get; set; }
    public long Size { get; set; }
    public required string Checksum { get; set; }
    public required StorageProviderType StorageProviderType { get; set; }
}
