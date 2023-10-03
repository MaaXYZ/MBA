using System.Text.Json;
using System.Text.Json.Nodes;
using MBA.Core.Data;

namespace MBA.Core.Managers;

public static class ConfigManager
{
    private static Serilog.ILogger Log => LogManager.Logger;

    static ConfigManager()
    {
        ConfigContext.InitOptions();
        InitConfig();
        LogManager.ConfigureLogger(Config.UI.DebugMode, true);
    }

    private static readonly object _configWriteLock = new();

    public static string AdbConfig { get; } = File.ReadAllText(GlobalInfo.AdbConfigFileFullPath);

    private static Config _config = new();
    public static Config Config
    {
        get => _config;
        private set
        {
            if (_config != value)
            {
                _config = value;
                ConfigChanged?.Invoke();
            }
        }
    }

    public delegate void ConfigChangedEventHandler();
    public static event ConfigChangedEventHandler? ConfigChanged;

    internal static void InitConfig()
    {
        var location = $"{nameof(ConfigManager)}.{nameof(InitConfig)}";

        TaskManager.RunTask(() =>
        {
            var configDir = GlobalInfo.ConfigFullPath;
            var configFilePath = GlobalInfo.ConfigFileFullPath;
            if (!Directory.Exists(configDir))
                _ = Directory.CreateDirectory(configDir);

            if (!File.Exists(configFilePath))
                SaveConfig();
            else
                LoadConfig();
        },
        location);
    }

    public static void LoadConfig()
    {
        var location = $"{nameof(ConfigManager)}.{nameof(LoadConfig)}";

        TaskManager.RunTask(() =>
        {
            Config = JsonSerializer.Deserialize(
                File.ReadAllText(GlobalInfo.ConfigFileFullPath),
                ConfigContext.Default.Config
            ) ?? Config;
        },
        location);
    }

    public static void SaveConfig()
    {
        var location = $"{nameof(ConfigManager)}.{nameof(SaveConfig)}";

        TaskManager.RunTask(() =>
        {
            lock (_configWriteLock)
            {
                File.WriteAllText(
                    GlobalInfo.ConfigFileFullPath,
                    JsonSerializer.Serialize(Config, ConfigContext.Default.Config)
                );
            }
        }
        , location);
    }

    public static void LogConfig()
    {
        var jsonObject = JsonNode.Parse(
            JsonSerializer.Serialize(Config, ConfigContext.Default.Config))
            as JsonObject ?? new();
        jsonObject.Remove(nameof(Config.Document));
        jsonObject.Remove(nameof(Config.UI));
        Log.Verbose("Current Config: {config}", jsonObject.ToString());
    }
}
