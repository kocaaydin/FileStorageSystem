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
                                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
                                .WriteTo.Console()
                                .WriteTo.File(hostContext.Configuration["StorageSettings:LogPath"]!.ToString(), rollingInterval: RollingInterval.Day)
                                .CreateLogger();

                    services.AddLogging(delegate (ILoggingBuilder loggingBuilder)
                    {
                        loggingBuilder.ClearProviders().AddSerilog(null, dispose: true);
                    });

                    services.Configure<StorageSettings>(hostContext.Configuration.GetSection("StorageSettings"));


                    services.AddEfDbContext<AppDbContext>(options =>
                    {
                        string connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection")!;
                        if (hostContext.HostingEnvironment.EnvironmentName == "Docker")
                        {
                            connectionString = connectionString.Replace("localhost", "db");
                        }
                        
                        options.UseNpgsql(connectionString);
                    });

                    services.AddTransient<IMetaDataRepository, MetaDataRepository>();
                    services.AddTransient<IChecksumCalculator, ChecksumCalculator>();
                    services.AddTransient<IChunkSizeCalculator, ChunkSizeCalculator>();

                    services.AddTransient<IMetaDataService, MetaDataService>();
                    services.AddTransient<IFileProcessService, FileProcessService>();
                    services.AddTransient<IProviderService, ProviderService>();
                    services.AddTransient<FileSystemStorageProvider>();
                    services.AddSingleton<StorageProviderFactory>();
                    services.AddAutoMapper(typeof(MetaDataProfile));

                });

    }
}