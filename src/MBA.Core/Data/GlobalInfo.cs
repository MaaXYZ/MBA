namespace MBA.Core.Data;

public static class GlobalInfo
{
    private const string Config = "./config";
    private const string ConfigFile = $"{Config}/config.json";
    private const string Debug = "./debug";
    private const string LogFile = $"{Debug}/mba.log";
    private const string Cache = "./cache";
    private const string Resource = "./Resource";

    internal static readonly string ConfigFullPath = Path.GetFullPath(Config);
    internal static readonly string ConfigFileFullPath = Path.GetFullPath(ConfigFile);
    internal static readonly string DebugFullPath = Path.GetFullPath(Debug);
    internal static readonly string LogFileFullPath = Path.GetFullPath(LogFile);
    internal static readonly string CacheFullPath = Path.GetFullPath(Cache);
    internal static readonly string ResourceFullPath = Path.GetFullPath(Resource);


    private const string BaseResource = $"{Resource}/Base";
    private const string AdbConfigFile = $"{Resource}/controller_config.json";

    internal static readonly string BaseResourceFullPath = Path.GetFullPath(BaseResource);
    internal static readonly string AdbConfigFileFullPath = Path.GetFullPath(AdbConfigFile);

    internal static readonly int ScreenshotHeight = 720;

    public static bool IsCli { get; set; }
}
