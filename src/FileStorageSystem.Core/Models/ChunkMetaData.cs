using FileStorageSystem.Core.Enums;

namespace FileStorageSystem.Core.Models;

public class ChunkMetaData : BaseModel
{
    public Guid FileMetaDataId { get; set; }
    public int ChunkIndex { get; set; }
    public long Size { get; set; }
    public required StorageProviderType StorageProviderType { get; set; }
    public required string Checksum { get; set; }
}
