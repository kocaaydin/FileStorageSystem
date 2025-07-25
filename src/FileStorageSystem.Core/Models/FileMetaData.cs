using FileStorageSystem.Core.Enums;

namespace FileStorageSystem.Core.Models;

public class FileMetaData: BaseModel
{
    public required string FileName { get; set; }
    public long OriginalSize { get; set; }
    public required string OriginalChecksum { get; set; }
    public DateTime UploadDate { get; set; }
    public required FileMetaDataStatus FileMetaDataStatus { get; set; }
    public ICollection<ChunkMetaData> ChunkMetaDatas { get; set; } = [];
}
