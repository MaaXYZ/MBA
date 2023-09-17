using MBA.Core.Enums;
using MaaToolKit.Extensions.Enums;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

namespace MBA.Core.Data;

public class Config
{
    public List<TaskType> Tasks { get; set; } = new() { TaskType.Daily };
    public List<TaskType> TasksExcept { get; set; } = new() { };
    public UIConfig UI { get; set; } = new();
    public CoreConfig Core { get; set; } = new();
    public GameConfig Game { get; set; } = new();
    public DailyConfig Daily { get; set; } = new();
    public WeeklyConfig Weekly { get; set; } = new();
    public ConfigDoc Doc { get; } = new();
}

public class UIConfig
{
    public string CurrentMBACoreVersion { get; set; } = "0.0.0";
    public bool FirstStartUp { get; set; } = true;
    public bool DebugMode { get; set; } = false;
    public string Proxy { get; set; } = string.Empty;

    [JsonIgnore]
    public Uri? ProxyUri => string.IsNullOrEmpty(Proxy)
        ? null
        : new Uri(Proxy.Contains("://") ? Proxy : $"http://{Proxy}");
}

public class CoreConfig
{
    public string Adb { get; set; } = "adb";
    public string AdbAddress { get; set; } = "127.0.0.1:5555";
    public AdbControllerType Touch { get; set; } = AdbControllerType.InputPresetAdb;
    public AdbControllerType ScreenCap { get; set; } = AdbControllerType.ScreenCapRawWithGzip;

    [JsonIgnore]
    public AdbControllerType ControlType => Touch | ScreenCap;
}

public class GameConfig
{
    public GameLanguageServer Language { get; set; } = GameLanguageServer.EN;
    public GameLanguageServer Server { get; set; } = GameLanguageServer.TaiwanHongKongMacao;
    public string PackageEntry { get; set; } = string.Empty;

    [JsonIgnore]
    public GameLanguageServer LanguageServer => Language | Server;
}

public class DailyConfig
{
    public uint TacticalChallengeTimes { get; set; } = 5;/*
    public bool DailyNormalMissionDuringActivities { get; set; } = false;
    public string NormalMissionId { get; set; } = "0-0";
    public uint NormalMissionTimes { get; set; } = 17;
    public string HardMissionId { get; set; } = "H0-0";
    public uint HardMissionTimes { get; set; } = 3;*/
    public string CommissionsId { get; set; } = "E";
    public uint CommissionsTimes { get; set; } = 1;

    [JsonIgnore]
    public DiffTaskDailyConfig DiffTask { get; }
    public DailyConfig() => DiffTask = new(this);
}

public class WeeklyConfig
{

}

public class DiffTaskDailyConfig
{
    private readonly DailyConfig _config;
    public DiffTaskDailyConfig(DailyConfig dailyConfig)
    {
        _config = dailyConfig;
    }

    public string TacticalChallenge => _config.TacticalChallengeTimes == 0
        ? TacticalChallenge0
        : TacticalChallenge1;
    private string TacticalChallenge0 => new JsonObject
    {
        ["diff_task"] = new JsonObject
        {
            ["Sub_Start_TacticalChallenge_Partial"] = new JsonObject
            {
                ["times_limit"] = _config.TacticalChallengeTimes
            },
            ["Sub_End_TacticalChallenge_Partial"] = new JsonObject
            {
                ["times_limit"] = _config.TacticalChallengeTimes
            },
        }
    }.ToString();
    private string TacticalChallenge1 => new JsonObject
    {
        ["diff_task"] = new JsonObject
        {
            ["Sub_Start_TacticalChallenge_Partial"] = new JsonObject
            {
                ["times_limit"] = _config.TacticalChallengeTimes
            },
        }
    }.ToString();

    public string Commissions => new JsonObject
    {
        ["diff_task"] = new JsonObject
        {
            ["Start_Commissions_Partial"] = new JsonObject
            {
                ["next"] = _config.CommissionsId == "E"
                         ? "Click_BaseDefense_Partial"
                         : "Click_ItemRetrieval_Partial"
            },
            ["Click_PlusButtons"] = new JsonObject
            {
                ["times_limit"] = _config.CommissionsTimes - 1
            },
        }
    }.ToString();
}

public class ConfigDoc
{
    public string Tasks { get; } = $"要执行的任务, {(int)TaskType.Bounty}.{TaskType.Bounty} (悬赏通缉), {(int)TaskType.Cafe}.{TaskType.Cafe} (咖啡厅), {(int)TaskType.Club}.{TaskType.Club} (社团), {(int)TaskType.Commissions}.{TaskType.Commissions} (特殊任务), {/*(int)TaskType.Crafting}.{TaskType.Crafting*/"  TODO"} (制造), {(int)TaskType.Mailbox}.{TaskType.Mailbox} (信箱), {(int)TaskType.Scrimmage}.{TaskType.Scrimmage} (学院交流会), {/*(int)TaskType.Shop}.{TaskType.Shop*/"  TODO"} (商店), {(int)TaskType.TacticalChallenge}.{TaskType.TacticalChallenge} (战术大赛), {(int)TaskType.Tasks}.{TaskType.Tasks} (任务，日常周常奖励收菜), {(int)TaskType.StartUp}.{TaskType.StartUp} (只启动游戏), {(int)TaskType.Daily}.{TaskType.Daily} (做日常), {(int)TaskType.Weekly}.{TaskType.Weekly} (做周常)";
    public string TasksExcept { get; } = $"要排除的任务 (高优先), 以下任务不可被排除: {(int)TaskType.Daily}.{TaskType.Daily}, {(int)TaskType.Weekly}.{TaskType.Weekly}, {(int)TaskType.StartUp}.{TaskType.StartUp}";

    #region UIConfig

    public string FirstStartUp { get; } = "是否第一次启动";
    public string DebugMode { get; } = "是否开启 Debug Mode (生成的文件会占用大量空间)";
    public string Proxy { get; } = "代理地址，例如 http://127.0.0.1:7890";

    #endregion

    #region CoreConfig

    public string Adb { get; } = "adb.exe 所在路径，相对绝对均可，例如 C:/adb.exe，不要有中文";
    public string AdbAddress { get; } = "adb 连接地址，例如 127.0.0.1:5555";
    public string Touch { get; } = $"点击方式：{AdbControllerType.InputPresetAdb}, {AdbControllerType.InputPresetMinitouch}, {AdbControllerType.InputPresetMaatouch}";
    public string ScreenCap { get; } = $"截图方式：{AdbControllerType.ScreenCapFastestWay}, {AdbControllerType.ScreenCapRawByNetcat}, {AdbControllerType.ScreenCapRawWithGzip}, {AdbControllerType.ScreenCapEncode}, {AdbControllerType.ScreenCapEncodeToFile}, {AdbControllerType.ScreenCapMinicapDirect}, {AdbControllerType.ScreenCapMinicapStream}";

    #endregion

    #region GameConfig

    public string Language { get; } = $"游戏内语言：{GameLanguageServer.EN}";
    // $"游戏内语言：{GameLanguageServer.JP}, {GameLanguageServer.KR}, {GameLanguageServer.EN}, {GameLanguageServer.TH}, {GameLanguageServer.TC}, {GameLanguageServer.SC}";

    public string Server { get; } = $"游戏服务器：{GameLanguageServer.Global}, {GameLanguageServer.Korea}, {GameLanguageServer.TaiwanHongKongMacao}, {GameLanguageServer.NorthAmerica}, {GameLanguageServer.Europe}, {GameLanguageServer.Asia}";
    // $"游戏服务器：{GameLanguageServer.Japanese}, {GameLanguageServer.Global}, {GameLanguageServer.Korea}, {GameLanguageServer.TaiwanHongKongMacao}, {GameLanguageServer.NorthAmerica}, {GameLanguageServer.Europe}, {GameLanguageServer.Asia}, {GameLanguageServer.Chinese}, {GameLanguageServer.YoStarCN}, {GameLanguageServer.Bilibili}, {GameLanguageServer.Ourplay}, {GameLanguageServer.OthersCN}";
    public string PackageEntry { get; set; } = "备用启动入口, 当 Language 和 Server 不匹配 或 Server 不支持时使用, 需要填入 activity, 例如 com.hypergryph.arknights/com.u8.sdk.U8UnityContext";

    #endregion

    #region DailyConfig

    public string TacticalChallengeTimes { get; } = "日活打 JJC 的次数, 0 ~ 5, 0 为只领取信用点";/*
    public string NormalMissionId { get; } = "普通任务的 Id, 0-0 为打目前打到的一关";
    public string NormalMissionTimes { get; } = "日活打普通任务的次数, > 0";
    public string HardMissionId { get; } = "困难任务的 Id, H0-0 为打目前打到的一关";
    public string HardMissionTimes { get; } = "日活打困难任务的次数, 1 ~ 3";*/
    public string CommissionsId { get; } = "特殊任务的 Id, Id 为 E (EXP items) 或 C (Credits)";
    public string CommissionsTimes { get; } = "日活打特殊任务的次数, > 0";

    #endregion
}
