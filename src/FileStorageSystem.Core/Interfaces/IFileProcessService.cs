namespace FileStorageSystem.Core.Interfaces;

public interface IFileProcessService
{
    Task UploadFilesAsync(List<string> filePaths);
    Task UploadFileAsync(string filePath);
    Task DownloadFileAsync(Guid fileId, string destinationPath);
    Task<bool> VerifyFileIntegrityAsync(Guid fileId);
    FileInfo GetFileInfo(string filePath);
}
