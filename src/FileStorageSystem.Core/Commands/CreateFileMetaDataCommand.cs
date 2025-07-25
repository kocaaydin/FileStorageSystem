using FileStorageSystem.Core.Dtos;
using FileStorageSystem.Core.Enums;

namespace FileStorageSystem.Core.Commands;

public class CreateFileMetaDataCommand: BaseDto
{
    public required string FileName { get; set; }
    public long OriginalSize { get; set; }
    public required string OriginalChecksum { get; set; }
    public DateTime UploadDate { get; set; }
    public required FileMetaDataStatus FileMetaDataStatus { get; set; }
    public List<CreateChunkMetaDataCommand> ChunkMetaDatas { get; set; } = [];
}
