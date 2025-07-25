namespace FileStorageSystem.Core.Dtos;

public class ChunkDataDto
{
    public required ChunkMetaDataDto ChunkMetaData { get; set; }
    public required Stream Data { get; set; }
}