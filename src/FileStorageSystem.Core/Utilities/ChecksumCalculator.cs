using System.Security.Cryptography;

namespace FileStorageSystem.Core.Helpers;

public class ChecksumCalculator : IChecksumCalculator
{
    public async Task<string> CalculateSha256Async(string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return await CalculateSha256Async(stream);
    }

    public async Task<string> CalculateSha256Async(Stream stream)
    {
        using var sha256 = SHA256.Create();
        stream.Seek(0, SeekOrigin.Begin);
        var hash = await sha256.ComputeHashAsync(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}

