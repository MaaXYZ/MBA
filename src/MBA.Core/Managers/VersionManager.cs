using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json.Nodes;
using MBA.Core.Data;

namespace MBA.Core.Managers;

public static class VersionManager
{
    private static Serilog.ILogger Log => LogManager.Logger;
    private static Config Config => ConfigManager.Config;

    public static string AssemblyVersion { get; private set; }
    public static string InformationalVersion { get; private set; }
    public static bool IsPreviewVersion { get; private set; } = false;
    public static bool Updated { get; private set; } = false;
    public static string ChangeLog { get; private set; } = "<failed to read>";
    public static bool Released { get; private set; } = false;

    static VersionManager()
    {
        AssemblyVersion = GetAssemblyVersion();
        InformationalVersion = GetInformationalVersion();
        if (InformationalVersion.EndsWith("-dev") || InformationalVersion.Contains("Preview"))
        {
            IsPreviewVersion = true;
        }

        SetUpdatedAndChangeLog();
        _ = SetReleasedAsync();
    }

    private static string GetAssemblyVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version?.ToString(3)
            ?? "<Unidentified>";
    }

    private static string GetInformationalVersion()
    {
        var attr = Attribute.GetCustomAttribute(
                Assembly.GetExecutingAssembly(),
                typeof(AssemblyInformationalVersionAttribute))
            as AssemblyInformationalVersionAttribute;
        return attr?.InformationalVersion
            ?? AssemblyVersion + "-<Unidentified>";
    }

    private static void SetUpdatedAndChangeLog()
    {
        if (Config.UI.CurrentMBACoreVersion == InformationalVersion)
            return;

        Config.UI.CurrentMBACoreVersion = InformationalVersion;
        if (IsPreviewVersion)
            return;

        Updated = true;
        try
        {
            var name = Assembly.GetExecutingAssembly().GetManifestResourceNames().First(x => x.Contains("CHANGELOG.md"))!;
            using var rs = Assembly.GetExecutingAssembly().GetManifestResourceStream(name)!;
            using StreamReader sr = new StreamReader(rs, detectEncodingFromByteOrderMarks: true);
            var changeLog = sr.ReadToEnd();

            if (string.IsNullOrWhiteSpace(changeLog))
                ChangeLog = "<NullOrWhiteSpace>";
            else
                ChangeLog = changeLog;
        }
        catch (Exception e)
        {
            Log.Warning("Failed to read ChangeLog: {Message}", e.Message);
        }
    }


    private static async Task SetReleasedAsync()
    {
        if (IsPreviewVersion)
            return;

        try
        {
            var latest = await GetLatestReleaseVersionAsync();
            if (latest != AssemblyVersion && latest != InformationalVersion)
            {
                var url = "https://github.com/MaaAssistantArknights/MBA/releases/latest";
                Released = true;
                Log.Information("New version released, available for download at {URL}.", url);
            }
        }
        catch (Exception e)
        {
            Log.Warning("VersionManager.{Func} failed: {Message}",
                nameof(GetLatestReleaseVersionAsync),
                e.Message);
            Log.Warning("Setting the {Proxy} in {Path} may be useful.",
                nameof(Config.UI.Proxy),
                GlobalInfo.ConfigFileFullPath);
        }
    }

    private static async Task<string> GetLatestReleaseVersionAsync()
    {
        var url = "https://api.github.com/repos/MaaAssistantArknights/MBA/releases/latest";
        var handler = new HttpClientHandler
        {
            AllowAutoRedirect = true,
        };
        if (Config.UI.ProxyUri != null)
        {
            handler.UseProxy = true;
            handler.Proxy = new WebProxy
            {
                Address = Config.UI.ProxyUri,
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false,
            };
        }

        var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
        client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        client.DefaultRequestHeaders.UserAgent.Add(ProductInfoHeaderValue.Parse("request"));
        var jsonString = await client.GetStringAsync(url);

        var jsonNode = JsonNode.Parse(jsonString);
        if (jsonNode == null)
            return string.Empty;

        jsonNode = jsonNode["tag_name"];
        if (jsonNode == null)
            return string.Empty;

        var tag = jsonNode.ToString();
        if (!tag.StartsWith('v'))
            return string.Empty;

        return tag.TrimStart('v');
    }
}
