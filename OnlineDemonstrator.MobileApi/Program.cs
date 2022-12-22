using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace OnlineDemonstrator.MobileApi
{
    public static class Program
    {
        [Obsolete("Obsolete")]
        public static void Main(string[] args)
        {
            IConfiguration configSerilog = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var pathToLog = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "crowded-place", "log", "serilog-log-.txt");

            var configuration = new LoggerConfiguration()
                .ReadFrom.Configuration(configSerilog)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("App", "crowded-place")
                .WriteTo.Console()
                .WriteTo.File(pathToLog, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 104857600, retainedFileCountLimit: 31);
            
            Log.Logger = configuration.CreateLogger();
            
            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        [Obsolete("Obsolete")]
        private static IWebHostBuilder CreateHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", true)
                .Build();
            var host = WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseConfiguration(config)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>();

            return host;
        }
    }
}