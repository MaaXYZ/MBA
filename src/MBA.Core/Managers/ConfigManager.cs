using System.Text.Json;
using MBA.Core.Data;
using System.Text.Json.Serialization;

namespace MBA.Core.Managers;

public static class ConfigManager
{
    private static Serilog.ILogger Log => LogManager.Logger;

    static ConfigManager()
    {
        _options.Converters.Add(new JsonStringEnumConverter());
        InitConfig();
        LogManager.ConfigureLogger(Config.UI.DebugMode, true);
    }

    private static readonly object _configWriteLock = new();

    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

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
            Config = JsonSerializer.Deserialize<Config>(
                File.ReadAllText(GlobalInfo.ConfigFileFullPath), _options
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
                    JsonSerializer.Serialize(Config, _options)
                );
            }
        }
        , location);
    }
}
