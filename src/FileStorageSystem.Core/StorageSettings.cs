namespace FileStorageSystem.Core;

public class StorageSettings
{
    public required string FileSystemStoragePath { get; set; }

    public required string DownloadedFiles { get; set; }

    public required string SampleFilesPath { get; set; }
}
