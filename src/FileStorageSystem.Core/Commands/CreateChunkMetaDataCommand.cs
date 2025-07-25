using FileStorageSystem.Core.Dtos;
using FileStorageSystem.Core.Enums;

namespace FileStorageSystem.Core.Commands;

public class CreateChunkMetaDataCommand: BaseDto
{
    public Guid FileMetaDataId { get; set; }
    public int ChunkIndex { get; set; }
    public long Size { get; set; }
    public required string Checksum { get; set; }

    public StorageProviderType? StorageProviderType { get; set; }
}