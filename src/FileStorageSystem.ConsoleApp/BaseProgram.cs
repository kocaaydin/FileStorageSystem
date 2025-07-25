using FileStorageSystem.Core;
using FileStorageSystem.Core.Interfaces;
using FileStorageSystem.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using FileStorageSystem.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using FileStorageSystem.Data.Repositories;
using FileStorageSystem.Core.Helpers;
using FileStorageSystem.Services;
using FileStorageSystem.StorageProviders;
using FileStorageSystem.Core.Mappers;

namespace FileStorageSystemConsoleApp
{
    public class BaseProgram
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    Log.Logger = new LoggerConfiguration()
                                .MinimumLevel.Information()
                                .WriteTo.Console()
                                .WriteTo.File("logs/app_log.txt", rollingInterval: RollingInterval.Day)
                                .CreateLogger();

                    services.AddLogging(delegate (ILoggingBuilder loggingBuilder)
                    {
                        loggingBuilder.ClearProviders().AddSerilog(null, dispose: true);
                    });

                    var storageSettingsSection = hostContext.Configuration.GetSection("StorageSettings");
                    services.Configure<StorageSettings>(storageSettingsSection);

                    if (!Directory.Exists(storageSettingsSection["FileSystemStoragePath"]))
                    {
                        Directory.CreateDirectory(storageSettingsSection["FileSystemStoragePath"]!);
                    }

                    services.AddEfDbContext<AppDbContext>(options =>
                    {
                        options.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection"));
                    });

                    services.AddTransient<IMetaDataRepository, MetaDataRepository>();
                    services.AddTransient<IChecksumCalculator, ChecksumCalculator>();
                    services.AddTransient<IChunkSizeCalculator, ChunkSizeCalculator>();

                    services.AddTransient<IChunkerService, ChunkerService>();
                    services.AddTransient<IMetaDataService, MetaDataService>();
                    services.AddTransient<IFileProcessService, FileProcessService>();
                    services.AddTransient<IProviderService, ProviderService>();
                    services.AddTransient<FileSystemStorageProvider>();
                    services.AddSingleton<StorageProviderFactory>();
                    services.AddAutoMapper(typeof(MetaDataProfile));

                });
        public static void CreateSampleFile(string filePath, int sizeInBytes)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir!);
            }

            var data = new byte[1024];
            var random = new Random();

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                long bytesWritten = 0;
                while (bytesWritten < sizeInBytes)
                {
                    random.NextBytes(data);
                    int bytesToWrite = (int)Math.Min(data.Length, sizeInBytes - bytesWritten);
                    fs.Write(data, 0, bytesToWrite);
                    bytesWritten += bytesToWrite;
                }
            }
        }
    }
}