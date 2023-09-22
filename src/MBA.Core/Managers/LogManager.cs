using MaaToolKit.Extensions;
using MBA.Core.Data;
using Serilog;

namespace MBA.Core.Managers;

public static class LogManager
{
    public static ILogger Logger { get; private set; } = Serilog.Core.Logger.None;

    static LogManager()
    {
        ConfigureLogger(true);
    }

    private static bool _logStarted = false;

    /// <summary>
    /// Configures the <see cref="Logger"/>.
    /// </summary>
    /// <param name="enableDebugMode">Indicating whether degub mode is enabled or not.</param>
    /// <param name="startLog">Indicating whether start infomation is logged or not.</param>
    public static void ConfigureLogger(bool enableDebugMode, bool startLog = false)
    {
        var env = Environment.GetEnvironmentVariable("MBA_ENVIRONMENT") ?? "Empty";
        enableDebugMode = enableDebugMode || env == "Debug";

        var config = new LoggerConfiguration()
            .WriteTo.File(
                path: GlobalInfo.LogFileFullPath,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}][{Level:u3}] {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7);

        config = GlobalInfo.IsCli
            ? config.WriteTo.Console()
            : config.WriteTo.Debug();

        config = enableDebugMode
            ? config.MinimumLevel.Verbose()
            : config.MinimumLevel.Information();
        SetFrameworkLog(enableDebugMode);

        Logger = config.CreateLogger();
        Log.Logger = Logger;

        if (startLog)
            LogStart(env, enableDebugMode);
    }

    private static void SetFrameworkLog(bool enableDebugMode)
    {
        MaaObject.FrameworkLogDir = GlobalInfo.DebugFullPath;
        MaaObject.DebugMode = enableDebugMode;
    }

    private static void LogStart(string env, bool enableDebugMode)
    {
        if (_logStarted) return;

        Log.Information("===================================");
        Log.Information("  MBA {UI} v{Version} started", GlobalInfo.IsCli ? "CLI" : "GUI", VersionManager.InformationalVersion);
        Log.Information("  Environment: {env}", env);
        Log.Information("  Debug Mode: {DebugMode}", enableDebugMode);
        /* Duplicate in famework log */
        // Log.Information("  User Dir: {CurrentDirectory}", Directory.GetCurrentDirectory());
        Log.Information("===================================");
    }
}
