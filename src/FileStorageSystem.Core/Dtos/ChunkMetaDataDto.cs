using FileStorageSystem.Core.Enums;

namespace FileStorageSystem.Core.Dtos;

public class ChunkMetaDataDto : BaseDto
{
    public Guid FileMetaDataId { get; set; }
    public int ChunkIndex { get; set; }
    public long Size { get; set; }
    public string? Checksum { get; set; }

    public StorageProviderType? StorageProviderType { get; set; }
}