using FileStorageSystem.Core.Interfaces;
using FileStorageSystem.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using FileStorageSystem.Core;

namespace FileStorageSystemConsoleApp
{
    class Program : BaseProgram
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Database.MigrateAsync();
            }

            var fileProcessor = host.Services.GetRequiredService<IFileProcessService>();
            var options = host.Services.GetRequiredService<IOptions<StorageSettings>>();

            List<string> filePaths =
            [
                Path.Combine(AppContext.BaseDirectory, options.Value.SampleFilesPath, "sample_file1.txt"),
                Path.Combine(AppContext.BaseDirectory, options.Value.SampleFilesPath, "sample_file2.txt")
            ];

            foreach (var filePath in filePaths)
            {
                if (!File.Exists(filePath))
                {
                    CreateSampleFile(filePath, 5 * 1024 * 1024);
                }
            }

            await fileProcessor.UploadFilesAsync(filePaths);

            Console.Write("Doysa Id Giriniz:");

            string uploadedFileIdString = Console.ReadLine()!;

            Guid uploadedFileGuid = Guid.Empty;

            if (!Guid.TryParse(uploadedFileIdString, out uploadedFileGuid))
            {
                Console.WriteLine("HATA: Geçersiz GUID girdiniz.");
                return;
            }

            if (uploadedFileGuid != Guid.Empty)
            {
                string downloadDestinationPath = Path.Combine(AppContext.BaseDirectory, options.Value.DownloadedFiles);
                Directory.CreateDirectory(downloadDestinationPath);
                
                await fileProcessor.DownloadFileAsync(uploadedFileGuid, downloadDestinationPath);

                Console.WriteLine("Dosya indirildi");   

                bool isIntegrityOk = await fileProcessor.VerifyFileIntegrityAsync(uploadedFileGuid);

                Console.WriteLine($"Dosya doğrulama sonucu: {(isIntegrityOk ? "Başarılı" : "Başarısız")}");
            }

            Console.WriteLine("Program sonlandı. Çıkmak için bir tuşa basın...");
            _ = Console.ReadLine()!;
        } 
    }
}