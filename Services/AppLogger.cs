using Serilog;
using System;
using System.IO;

namespace FGC_Stat_Analyzer_wpf.Services
{
    public static class AppLogger
    {
        public static void Initialize()
        {
            string logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PotMonster", "Logs");

            Directory.CreateDirectory(logDirectory);

            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File(Path.Combine(logDirectory, "log-.txt"), rollingInterval:RollingInterval.Day, retainedFileCountLimit: 10, shared: true,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}").CreateLogger();

            Log.Information("====================================================");
            Log.Information("Application Logging initialized");
            Log.Information("Application version: {Version}", typeof(AppLogger).Assembly.GetName().Version);
            Log.Information("Log directory: {LogDirectory}", logDirectory);
        }

        public static void Shutdown()
        {
            Log.Information("Application shutting down");
            Log.CloseAndFlush();
        }
    }
}
