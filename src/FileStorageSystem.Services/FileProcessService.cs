using AutoMapper;
using FileStorageSystem.Core.Dtos;
using FileStorageSystem.Core.Enums;
using FileStorageSystem.Core.Exceptions;
using FileStorageSystem.Core.Helpers;
using FileStorageSystem.Core.Interfaces;
using FileStorageSystem.StorageProviders;
using Microsoft.Extensions.DependencyInjection;

namespace FileStorageSystem.Services;

public class FileProcessService(
        IMetaDataService metaDataService,
        StorageProviderFactory storageProviderFactory,
        IProviderService providerService,
        IChecksumCalculator checksumCalculator,
        IServiceProvider serviceProvider,
        IChunkSizeCalculator chunkSizeCalculator) : IFileProcessService
{

    public async Task UploadFilesAsync(List<string> filePaths)
    {
        if (!(filePaths?.Count > 0))
        {
            throw new ArgumentException("File paths cannot be null or empty.", nameof(filePaths));
        }

        await Task.WhenAll(filePaths.Select(async path =>
        {
            using var scope = serviceProvider.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<IFileProcessService>();
            await processor.UploadFileAsync(path);
        }));
    }

    public async Task UploadFileAsync(string filePath)
    {
        // Detaylandırılmak istenilirse yarım kalan dosya yükleme işlemi kaldığı yerden devam etsin algoritması
        // eklenebilir.
        var status = FileMetaDataStatus.InProgress;

        var fileMetadata = await CreateFileMetaDataAsync(filePath);

        try
        {
            await ChunksProcessAsync(filePath, fileMetadata);
            status = FileMetaDataStatus.Completed;
        }
        catch
        {
            status = FileMetaDataStatus.Failed;
            throw;
        }
        finally
        {
            await metaDataService.UpdateFileMetaDataStatusAsync(fileMetadata.Id, status);
        }
    }

    private async Task ChunksProcessAsync(string filePath, FileMetaDataDto fileMetadata)
    {
        int currentProviderIndex = 0;
        var providerDefinitions = providerService.GetAvailableProviders();
        //yield return kullanıldı performans açısından. veya tüm parçaları böl aynı anda yükle/kaydette yapılabilir. 
        //tamamını aynı anda yükleme ile birlikte hata veren chunklar tespit edilip farklıı sağlayıcılara yönlendirme yapılabilir.
        //Bu sayede bir sağlayıcıda hata oluştuğunda diğer sağlayıcılara yükleme işlemi devam edebilir.
        await foreach (var chunk in GetChunksAsync(filePath))
        {
            var providerType = providerDefinitions[currentProviderIndex % providerDefinitions.Count];
            var storageProvider = storageProviderFactory.GetProvider(providerType);

            //Buraya hata durumunda bir sonraki sağlayıcıya geçme mantığı eklenebilir.
            //Bir sonraki sağlayıcıya eklenip chunkmetadatası ona göre kaydedilir.
            await storageProvider.SaveChunkAsync(chunk.ChunkMetaData.Id, chunk.Data, fileMetadata.FileName);

            chunk.ChunkMetaData.FileMetaDataId = fileMetadata.Id;
            chunk.ChunkMetaData.StorageProviderType = providerType;

            //Entity framework için retry eklenenebilir.
            await metaDataService.AddChunkMetaDataAsync(chunk.ChunkMetaData);

            currentProviderIndex++;
        }
    }

    public FileInfo GetFileInfo(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }
        return fileInfo;
    }


    private async Task<FileMetaDataDto> CreateFileMetaDataAsync(string filePath)
    {
        var fileInfo = GetFileInfo(filePath);

        var fileMetadata = new FileMetaDataDto
        {
            Id = Guid.NewGuid(),
            FileName = fileInfo.Name,
            OriginalSize = fileInfo.Length,
            OriginalChecksum = await checksumCalculator.CalculateSha256Async(filePath),
            UploadDate = DateTime.UtcNow,
            FileMetaDataStatus = FileMetaDataStatus.InProgress
        };

        await metaDataService.AddFileMetaDataAsync(fileMetadata);
        return fileMetadata;
    }

    public async Task DownloadFileAsync(Guid fileId, string destinationPath)
    {
        var fileMetadata = await metaDataService.GetFileMetaDataWithChunksAsync(fileId);

        if (fileMetadata == null)
        {
            throw new FileNotFoundException($"File with ID {fileId} not found.");
        }

        using (var mergedStream = await MergeFileAsync(fileMetadata))
        {
            var mergedChecksum = await checksumCalculator.CalculateSha256Async(mergedStream);
            if (mergedChecksum != fileMetadata.OriginalChecksum)
            {
                throw new FileValidationException($"Dosya doğrulama işleminde hata oluştu. FileId: {fileId}.");
            }
            mergedStream.Seek(0, SeekOrigin.Begin);
            var outputFilePath = Path.Combine(destinationPath, fileMetadata.FileName);

            using (var outputFileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                await mergedStream.CopyToAsync(outputFileStream);
            }
        }
    }

    public async Task<bool> VerifyFileIntegrityAsync(Guid fileId)
    {
        try
        {
            var fileMetadata = await metaDataService.GetFileMetaDataWithChunksAsync(fileId);
            if (fileMetadata == null)
            {
                return false;
            }

            using (var mergedStream = await MergeFileAsync(fileMetadata))
            {
                var mergedChecksum = await checksumCalculator.CalculateSha256Async(mergedStream!);
                bool isIntegrityOk = (mergedChecksum == fileMetadata.OriginalChecksum);
                return isIntegrityOk;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private async IAsyncEnumerable<ChunkDataDto> GetChunksAsync(string filePath)
    {
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var buffer = new byte[chunkSizeCalculator.CalculateOptimalChunkSize(fileStream.Length)];
        int bytesRead;
        int chunkIndex = 0;

        while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            var chunkStream = new MemoryStream(buffer, 0, bytesRead, writable: false, publiclyVisible: true);

            var chunkChecksum = await checksumCalculator.CalculateSha256Async(chunkStream);

            chunkStream.Seek(0, SeekOrigin.Begin);

            yield return new ChunkDataDto
            {
                ChunkMetaData = new()
                {
                    Id = Guid.NewGuid(),
                    ChunkIndex = chunkIndex,
                    Size = bytesRead,
                    Checksum = chunkChecksum
                },
                Data = chunkStream
            };

            chunkIndex++;
        }
    }

    private async Task<Stream> MergeFileAsync(FileMetaDataDto fileMetaData)
    {
        var sortedChunks = fileMetaData.ChunkMetaDatas.OrderBy(c => c.ChunkIndex).ToList();
        var memoryStream = new MemoryStream();

        foreach (var chunk in sortedChunks)
        {
            var providerType = (StorageProviderType)Enum.Parse(typeof(StorageProviderType), chunk.StorageProviderType.ToString()!);
            var storageProvider = storageProviderFactory.GetProvider(providerType);
            using var chunkStream = await storageProvider.GetChunkAsync(chunk.Id, fileMetaData.FileName);

            await chunkStream.CopyToAsync(memoryStream);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }
}
