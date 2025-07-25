namespace FileStorageSystem.Core.Helpers;


// checker ile birleştirilip core tarafına taşınabilir
public class ChunkSizeCalculator : IChunkSizeCalculator
{
    public int CalculateOptimalChunkSize(long fileSizeInBytes)
    {
        const int minChunkSize = 20 * 1024;
        const int maxChunkSize = 8 * 1024 * 1024;

        const int targetChunkCount = 16;

        long optimal = fileSizeInBytes / targetChunkCount;

        if (optimal < minChunkSize) return minChunkSize;
        if (optimal > maxChunkSize) return maxChunkSize;

        return (int)optimal;
    }
}