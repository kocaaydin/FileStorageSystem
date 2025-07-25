namespace FileStorageSystem.Core.Helpers;

public interface IChunkSizeCalculator
{
    int CalculateOptimalChunkSize(long fileSizeInBytes);
}
