namespace FileStorageSystem.Core.Helpers;

public interface IChecksumCalculator
{
    Task<string> CalculateSha256Async(string filePath);
    Task<string> CalculateSha256Async(Stream stream);
}

