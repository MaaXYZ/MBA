using MaaToolKit.Extensions;
using MBA.Core.Data;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace MBA.Core.Managers;

public static class LogManager
{
    public static ILogger Logger { get; private set; } = Serilog.Core.Logger.None;

    private static readonly LoggingLevelSwitch _levelSwitch = new();

    static LogManager()
    {
        const string Template = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}][{Level:u3}] {Message:lj}{NewLine}{Exception}";
        var config = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(_levelSwitch)
            .WriteTo.File(
                path: GlobalInfo.LogFileFullPath,
                outputTemplate: Template,
                // levelSwitch: LevelSwitch,
                restrictedToMinimumLevel: LogEventLevel.Verbose,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7);

        config = GlobalInfo.IsCli
            ? config.WriteTo.Console()
            : config.WriteTo.Debug();

        Logger = config.CreateLogger();
        Log.Logger = Logger;

        ConfigureLogger(true);
    }

    /// <summary>
    ///     Configures the <see cref="Logger"/>.
    /// </summary>
    /// <param name="enableDebugMode">Indicating whether degub mode is enabled or not.</param>
    /// <param name="startLog">Indicating whether start infomation is logged or not.</param>
    public static void ConfigureLogger(bool enableDebugMode, bool startLog = false)
    {
        // Information  - 给用户看的信息
        // Debug        - 给开发者看的信息
        // Verbose      - 跟踪信息，开发者不怎么想看的那种

        var env = Environment.GetEnvironmentVariable("MBA_ENVIRONMENT") ?? "Empty";
        enableDebugMode = enableDebugMode || env == "Debug";
        SetFrameworkLog(enableDebugMode);

        if (enableDebugMode)
            _levelSwitch.MinimumLevel = LogEventLevel.Verbose;
        else
            _levelSwitch.MinimumLevel = LogEventLevel.Information;

        if (startLog)
            LogStart(env, enableDebugMode);
    }

    private static void SetFrameworkLog(bool enableDebugMode)
    {
        MaaObject.FrameworkLogDir = GlobalInfo.DebugFullPath;
        MaaObject.DebugMode = enableDebugMode;
    }

    private static bool _logStarted = false;

    private static void LogStart(string env, bool enableDebugMode)
    {
        if (_logStarted) return;
        _logStarted = true;

        Log.Information("===================================");
        Log.Information("  MBA {UI} v{Version} started", GlobalInfo.IsCli ? "CLI" : "GUI", VersionManager.InformationalVersion);
        Log.Information("  Environment: {env}", env);
        Log.Information("  Debug Mode: {DebugMode}", enableDebugMode);
        /* Duplicate in famework log */
        // Log.Information("  User Dir: {CurrentDirectory}", Directory.GetCurrentDirectory());
        Log.Information("===================================");
    }
}
