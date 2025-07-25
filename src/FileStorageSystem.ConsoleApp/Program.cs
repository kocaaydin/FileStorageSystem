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

            await FileProcessAsync(host);

            Console.WriteLine("Program sonlandı. Çıkmak için bir tuşa basın...");

            _ = Console.ReadLine()!;
        }

        private static async Task<bool> FileProcessAsync(Microsoft.Extensions.Hosting.IHost host)
        {
            var fileProcessor = host.Services.GetRequiredService<IFileProcessService>();
            var options = host.Services.GetRequiredService<IOptions<StorageSettings>>();

            Console.WriteLine("Yüklemek istediğiniz dosyaların adreslerini virgül ile ayırarak giriniz (örneğin: C:\\dosya1.txt,C:\\dosya2.txt):");


            List<string> filePaths = [.. Console.ReadLine()!
                .Split(',')
                .Select(path => path.Trim())
                .Where(path => !string.IsNullOrEmpty(path) && File.Exists(path))];
            

            await fileProcessor.UploadFilesAsync(filePaths);

            Console.Write("Doysa Id Giriniz:");

            string uploadedFileIdString = Console.ReadLine()!;

            Guid uploadedFileGuid = Guid.Empty;

            if (!Guid.TryParse(uploadedFileIdString, out uploadedFileGuid))
            {
                Console.WriteLine("HATA: Geçersiz GUID girdiniz.");
                return false;
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

            return true;
        }
    }
}