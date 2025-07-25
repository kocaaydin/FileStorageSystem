using FileStorageSystem.Core.Enums;

namespace FileStorageSystem.Core.Dtos;

public class FileMetaDataDto : BaseDto
{
    public required string FileName { get; set; }
    public long OriginalSize { get; set; }
    public required string OriginalChecksum { get; set; }
    public DateTime UploadDate { get; set; }
    public required FileMetaDataStatus FileMetaDataStatus { get; set; }
    public List<ChunkMetaDataWithProviderDto> Chunks { get; set; } = [];
}
