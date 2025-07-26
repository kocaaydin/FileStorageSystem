using FileStorageSystem.Core.Dtos;
using FileStorageSystem.Core.Enums;
using FileStorageSystem.Core.Exceptions;
using FileStorageSystem.Core.Helpers;
using FileStorageSystem.Core.Interfaces;
using FileStorageSystem.StorageProviders;
using Microsoft.Extensions.Logging;
using Moq;


namespace FileStorageSystem.Services.Tests;

[TestFixture]
public class FileProcessServiceTests
{
    private Mock<IMetaDataService> _metaDataServiceMock;
    private Mock<StorageProviderFactory> _storageProviderFactoryMock;
    private Mock<IProviderService> _providerServiceMock;
    private Mock<IChecksumCalculator> _checksumCalculatorMock;
    private Mock<IServiceProvider> _serviceProviderMock;
    private Mock<IChunkSizeCalculator> _chunkSizeCalculatorMock;
    private Mock<ILogger<FileProcessService>> _loggerMock;
    private Mock<IStorageProvider> _storageProviderMock;

    private FileProcessService _fileProcessService;

    [SetUp]
    public void Setup()
    {
        _metaDataServiceMock = new Mock<IMetaDataService>();
        _providerServiceMock = new Mock<IProviderService>();
        _checksumCalculatorMock = new Mock<IChecksumCalculator>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _chunkSizeCalculatorMock = new Mock<IChunkSizeCalculator>();
        _loggerMock = new Mock<ILogger<FileProcessService>>();
        _storageProviderMock = new Mock<IStorageProvider>();

        _storageProviderFactoryMock = new Mock<StorageProviderFactory>(null);
        _storageProviderFactoryMock
            .Setup(f => f.GetProvider(It.IsAny<StorageProviderType>()))
            .Returns(_storageProviderMock.Object);

        _providerServiceMock
            .Setup(p => p.GetAvailableProviders())
            .Returns([StorageProviderType.FileSystem]);

        _chunkSizeCalculatorMock
            .Setup(c => c.CalculateOptimalChunkSize(It.IsAny<long>()))
            .Returns(1024);

        _checksumCalculatorMock
            .Setup(c => c.CalculateSha256Async(It.IsAny<string>()))
            .ReturnsAsync("checksum123");

        _checksumCalculatorMock
            .Setup(c => c.CalculateSha256Async(It.IsAny<Stream>()))
            .ReturnsAsync("checksum123");

        _fileProcessService = new FileProcessService(
            _metaDataServiceMock.Object,
            _storageProviderFactoryMock.Object,
            _providerServiceMock.Object,
            _checksumCalculatorMock.Object,
            _serviceProviderMock.Object,
            _chunkSizeCalculatorMock.Object,
            _loggerMock.Object
        );
    }


    [Test]
    public void GetFileInfo_ReturnsFileInfo_WhenFileExists()
    {
        string tempFile = Path.GetTempFileName();

        var info = _fileProcessService.GetFileInfo(tempFile);

        Assert.Multiple(() =>
        {
            Assert.That(info.Exists, Is.True);
            Assert.That(info.Length, Is.GreaterThanOrEqualTo(0));
        });

        File.Delete(tempFile);
    }


    [Test]
    public void UploadFilesAsync_ShouldThrow_WhenFileListIsEmpty()
    {
        Assert.ThrowsAsync<ArgumentException>(() => _fileProcessService.UploadFilesAsync([]));
    }

    [Test]
    public async Task UploadFileAsync_ShouldUpdateMetaDataStatusCompleted_WhenSuccess()
    {
        var tempFile = Path.GetTempFileName();
        await File.WriteAllTextAsync(tempFile, "Test Data");

        _metaDataServiceMock
            .Setup(m => m.AddFileMetaDataAsync(It.IsAny<FileMetaDataDto>()))
            .Returns(Task.CompletedTask);

        _metaDataServiceMock
            .Setup(m => m.UpdateFileMetaDataStatusAsync(It.IsAny<Guid>(), It.IsAny<FileMetaDataStatus>()))
            .Returns(Task.CompletedTask);

        _storageProviderMock
            .Setup(s => s.SaveChunkAsync(It.IsAny<Guid>(), It.IsAny<Stream>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _metaDataServiceMock
            .Setup(m => m.AddChunkMetaDataAsync(It.IsAny<ChunkMetaDataDto>()))
            .Returns(Task.CompletedTask);

        await _fileProcessService.UploadFileAsync(tempFile);

        _metaDataServiceMock.Verify(m => m.UpdateFileMetaDataStatusAsync(It.IsAny<Guid>(), FileMetaDataStatus.Completed), Times.Once);

        File.Delete(tempFile);
    }


    [Test]
    public void DownloadFileAsync_ShouldThrow_WhenChecksumFails()
    {
        var fileId = Guid.NewGuid();

        _metaDataServiceMock
            .Setup(m => m.GetFileMetaDataWithChunksAsync(fileId))
            .ReturnsAsync(new FileMetaDataDto
            {
                Id = fileId,
                FileName = "test.txt",
                OriginalChecksum = "checksum",
                ChunkMetaDatas =
                [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        ChunkIndex = 0,
                        StorageProviderType =
                        StorageProviderType.FileSystem,
                        Checksum = "checksum123"
                    }
                ]
            });

        _storageProviderMock
            .Setup(s => s.GetChunkAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync(new MemoryStream([1, 2, 3]));

        _checksumCalculatorMock
            .Setup(c => c.CalculateSha256Async(It.IsAny<Stream>()))
            .ReturnsAsync("checksum1234");

        Assert.ThrowsAsync<FileValidationException>(() => _fileProcessService.DownloadFileAsync(fileId, Path.GetTempPath()));
    }

    [Test]
    public async Task VerifyFileIntegrityAsync_ShouldReturnTrue_WhenChecksumsMatch()
    {
        var fileId = Guid.NewGuid();

        _metaDataServiceMock
            .Setup(m => m.GetFileMetaDataWithChunksAsync(fileId))
            .ReturnsAsync(new FileMetaDataDto
            {
                Id = fileId,
                FileName = "test.txt",
                OriginalChecksum = "checksum123",
                ChunkMetaDatas =
                [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        ChunkIndex = 0,
                        StorageProviderType = StorageProviderType.FileSystem
                    }
                ]
            });

        _storageProviderMock
            .Setup(s => s.GetChunkAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync(new MemoryStream([1, 2, 3]));

        var result = await _fileProcessService.VerifyFileIntegrityAsync(fileId);

        Assert.That(result, Is.True);
    }
}