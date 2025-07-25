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
            var options = host.Services.GetRequiredService<IOptions<StorageSettings>>();

            await Init(host, options);

            await FileProcessAsync(host, options);

            Console.WriteLine("Program sonlandı. Çıkmak için bir tuşa basın...");

            _ = Console.ReadLine()!;
        }

        private static async Task Init(Microsoft.Extensions.Hosting.IHost host, IOptions<StorageSettings> options)
        {
            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Database.MigrateAsync();
            }

            if (!Directory.Exists(options.Value.ChunksPath))
            {
                Directory.CreateDirectory(options.Value.ChunksPath);
            }

            if (!Directory.Exists(options.Value.DownloadedFiles))
            {
                Directory.CreateDirectory(options.Value.DownloadedFiles);
            }
        }

        private static async Task<bool> FileProcessAsync(Microsoft.Extensions.Hosting.IHost host, IOptions<StorageSettings> options)
        {
            var fileProcessor = host.Services.GetRequiredService<IFileProcessService>();

            while (true)
            {
                Console.WriteLine("Dosya yüklemek için 'Y' tuşuna basın, çıkmak için 'Q' tuşuna basın.");
                var key = Console.ReadLine();

                if (key.ToLower() == "q")
                {
                    return false;
                }

                if (key.ToLower() == "y")
                {
                    await FileUploadAsync(fileProcessor);

                    await FileDownloadAsync(options, fileProcessor);
                }
            }
        }

        private static async Task FileDownloadAsync(IOptions<StorageSettings> options, IFileProcessService fileProcessor)
        {
            Console.Write("İndirmek istediğiniz dosyanın GUID'sini giriniz: ");

            string uploadedFileIdString = Console.ReadLine()!;

            Guid uploadedFileGuid = Guid.Empty;

            if (!Guid.TryParse(uploadedFileIdString, out uploadedFileGuid))
            {
                Console.WriteLine("HATA: Geçersiz GUID girdiniz.");
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
        }

        private static async Task FileUploadAsync(IFileProcessService fileProcessor)
        {
            Console.WriteLine("Yüklemek istediğiniz dosyaların adreslerini virgül ile ayırarak giriniz (örneğin: C:\\dosya1.txt,C:\\dosya2.txt):");

            List<string> filePaths = [.. Console.ReadLine()!
                                .Split(',')
                                .Select(path => path.Trim())
                                .Where(path => !string.IsNullOrEmpty(path) && File.Exists(path))];

            await fileProcessor.UploadFilesAsync(filePaths);
        }
    }
}