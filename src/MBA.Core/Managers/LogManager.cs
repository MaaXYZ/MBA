using MaaToolKit.Extensions.ComponentModel;
using MBA.Core.Data;
using Serilog;

namespace MBA.Core.Managers;

public static class LogManager
{
    public static ILogger Logger { get; private set; }

    static LogManager()
    {
        var env = Environment.GetEnvironmentVariable("MBA_ENVIRONMENT") ?? "Empty";
        var isDebugMode = ConfigManager.Config.UI.DebugMode || env == "Debug";
        var config = new LoggerConfiguration()
            .WriteTo.File(
                path: GlobalInfo.LogFileFullPath,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}][{Level:u3}] {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true);

        config = GlobalInfo.IsCli
            ? config.WriteTo.Console()
            : config.WriteTo.Debug();

        config = isDebugMode
            ? config.MinimumLevel.Verbose()
            : config.MinimumLevel.Information();
        SetFrameworkLog(isDebugMode);

        Logger = config.CreateLogger();
        LogStart(env, isDebugMode);
    }

    private static void SetFrameworkLog(bool isDebugMode)
    {
        MaaObject.FrameworkLogDir = GlobalInfo.DebugFullPath;
        MaaObject.DebugMode = isDebugMode;
    }

    private static void LogStart(string env, bool isDebugMode)
    {
        Log.Logger = Logger;
        Log.Information("===================================");
        Log.Information("  MBA {UI} started", GlobalInfo.IsCli ? "CLI" : "GUI");
        Log.Information("  Environment: {MBA_ENVIRONMENT}", env);
        Log.Information("  Debug Mode: {DebugMode}", isDebugMode);
        Log.Information("  User Dir: {CurrentDirectory}", Directory.GetCurrentDirectory());
        Log.Information("===================================");
    }
}
