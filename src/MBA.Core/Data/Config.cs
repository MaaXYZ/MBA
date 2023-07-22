using MBA.Core.Enums;
using MaaToolKit.Extensions.Enums;
using System.Text.Json.Serialization;

namespace MBA.Core.Data;

public class Config
{
    public List<TaskType> Tasks { get; set; } = new() { TaskType.Daily };
    public List<TaskType> TasksExcept { get; set; } = new() { };
    public string Tasks_Doc { get; } = $"要执行的任务, {(int)TaskType.Bounty}.{TaskType.Bounty} (悬赏通缉), {(int)TaskType.Cafe}.{TaskType.Cafe} (咖啡厅), {(int)TaskType.Club}.{TaskType.Club} (社团), {(int)TaskType.Commissions}.{TaskType.Commissions} (特殊任务), {(int)TaskType.Crafting}.{TaskType.Crafting} (制造), {(int)TaskType.Mailbox}.{TaskType.Mailbox} (信箱), {(int)TaskType.Scrimmage}.{TaskType.Scrimmage} (学院交流会), {(int)TaskType.Shop}.{TaskType.Shop} (商店), {(int)TaskType.TacticalChallenge}.{TaskType.TacticalChallenge} (战术大赛), {(int)TaskType.Tasks}.{TaskType.Tasks} (任务，日常周常奖励收菜), {(int)TaskType.Daily}.{TaskType.Daily} (做日常), {(int)TaskType.Weekly}.{TaskType.Weekly} (做周常), {(int)TaskType.StartUp}.{TaskType.StartUp} (只启动游戏)";
    public string TasksExcept_Doc { get; } = $"要排除的任务 (高优先), 以下任务不可被排除: {(int)TaskType.Daily}.{TaskType.Daily}, {(int)TaskType.Weekly}.{TaskType.Weekly}, {(int)TaskType.StartUp}.{TaskType.StartUp}";

    public UIConfig UI { get; set; } = new();
    public CoreConfig Core { get; set; } = new();
    public GameConfig Game { get; set; } = new();
    public DailyConfig Daily { get; set; } = new();
    public WeeklyConfig Weekly { get; set; } = new();
}

public class UIConfig
{
    public bool FirstStartUp { get; set; } = true;
    public bool DebugMode { get; set; } = false;
    public string DebugMode_Doc { get; } = "是否开启 Debug Mode (生成的文件会占用大量空间)";
}

public class CoreConfig
{
    public string Adb { get; set; } = "adb";
    public string Adb_Doc { get; } = "adb.exe 所在路径，相对绝对均可，例如 C:/adb.exe，不要有中文";

    public string AdbAddress { get; set; } = "127.0.0.1:5555";
    public string AdbAddress_Doc { get; } = "adb 连接地址，例如 127.0.0.1:5555";

    public AdbControllerType Touch { get; set; } = AdbControllerType.InputPresetAdb;
    public string Touch_Doc { get; } = $"点击方式：{AdbControllerType.InputPresetAdb}, {AdbControllerType.InputPresetMinitouch}, {AdbControllerType.InputPresetMaatouch}";

    public AdbControllerType ScreenCap { get; set; } = AdbControllerType.ScreenCapRawWithGzip;
    public string ScreenCap_Doc { get; } = $"截图方式：{AdbControllerType.ScreenCapFastestWay}, {AdbControllerType.ScreenCapRawByNetcat}, {AdbControllerType.ScreenCapRawWithGzip}, {AdbControllerType.ScreenCapEncode}, {AdbControllerType.ScreenCapEncodeToFile}, {AdbControllerType.ScreenCapMinicapDirect}, {AdbControllerType.ScreenCapMinicapStream}";

    [JsonIgnore]
    public AdbControllerType ControlType => Touch | ScreenCap;
}

public class GameConfig
{
    public GameLanguageServer Language { get; set; } = GameLanguageServer.EN;
    public string Language_Doc { get; } = $"游戏内语言：{GameLanguageServer.EN}";
    // $"游戏内语言：{GameLanguageServer.JP}, {GameLanguageServer.KR}, {GameLanguageServer.EN}, {GameLanguageServer.TH}, {GameLanguageServer.TC}, {GameLanguageServer.SC}";

    public GameLanguageServer Server { get; set; } = GameLanguageServer.TaiwanHongKongMacao;
    public string Server_Doc { get; } = $"游戏服务器：{GameLanguageServer.Global}, {GameLanguageServer.Korea}, {GameLanguageServer.TaiwanHongKongMacao}, {GameLanguageServer.NorthAmerica}, {GameLanguageServer.Europe}, {GameLanguageServer.Asia}";
    // $"游戏服务器：{GameLanguageServer.Japanese}, {GameLanguageServer.Global}, {GameLanguageServer.Korea}, {GameLanguageServer.TaiwanHongKongMacao}, {GameLanguageServer.NorthAmerica}, {GameLanguageServer.Europe}, {GameLanguageServer.Asia}, {GameLanguageServer.Chinese}, {GameLanguageServer.YoStarCN}, {GameLanguageServer.Bilibili}, {GameLanguageServer.Ourplay}, {GameLanguageServer.OthersCN}";

    [JsonIgnore]
    public GameLanguageServer LanguageServer => Language | Server;

    public string PackageEntry { get; set; } = string.Empty;

    public string PackageEntry_Doc { get; set; } = "备用启动入口, 当 Language 和 Server 不匹配 或 Server 不支持时使用, 需要填入 activity, 例如 com.hypergryph.arknights/com.u8.sdk.U8UnityContext";
}

public class DailyConfig
{
    public uint TacticalChallengeTimes { get; set; } = 5;
    public string TacticalChallengeTimes_Doc { get; } = "日活打 JJC 的次数, 0 ~ 5, 0 为只领取信用点";

    public bool DailyNormalMissionDuringActivities { get; set; } = false;
    public string NormalMissionId { get; set; } = "0-0";
    public uint NormalMissionTimes { get; set; } = 17;
    public string NormalMissionId_Doc { get; } = "普通任务的 Id, 0-0 为打目前打到的一关";
    public string NormalMissionTimes_Doc { get; } = "日活打普通任务的次数, > 0";

    public string HardMissionId { get; set; } = "H0-0";
    public uint HardMissionTimes { get; set; } = 3;
    public string HardMissionId_Doc { get; } = "困难任务的 Id, H0-0 为打目前打到的一关";
    public string HardMissionTimes_Doc { get; } = "日活打困难任务的次数, 1 ~ 3";

    public string CommissionsId { get; set; } = "E";
    public uint CommissionsTimes { get; set; } = 1;
    public string CommissionsId_Doc { get; } = "特殊任务的 Id, Id 为 E (EXP items) 或 C (Credits)";
    public string CommissionsTimes_Doc { get; } = "日活打特殊任务的次数, > 0";

    public DailyConfig()
    {
        DiffTask = new(this);
    }

    [JsonIgnore]
    public DiffTaskConfig DiffTask { get; }

    public class DiffTaskConfig
    {
        private readonly DailyConfig _c;
        public DiffTaskConfig(DailyConfig dailyConfig)
            => _c = dailyConfig;

        public string TacticalChallenge => _c.TacticalChallengeTimes == 0 ? TacticalChallenge0 : TacticalChallenge1;
        private string TacticalChallenge0 => $@"{{ ""diff_task"": {{
            ""Sub_Start_TacticalChallenge_Partial"": {{ ""times_limit"": {_c.TacticalChallengeTimes} }},
            ""Sub_End_TacticalChallenge_Partial"": {{ ""times_limit"": {_c.TacticalChallengeTimes} }}
            }} }}";
        private string TacticalChallenge1 => $@"{{ ""diff_task"": {{
            ""Sub_Start_TacticalChallenge_Partial"": {{ ""times_limit"": {_c.TacticalChallengeTimes} }}
            }} }}";

        public string Commissions => $@"{{ ""diff_task"": {{
            ""Start_Commissions_Partial"": {{ ""next"": ""{(_c.CommissionsId == "E" ? "Click_BaseDefense_Partial" : "Click_ItemRetrieval_Partial")}"" }},
            ""Click_PlusButtons"": {{ ""times_limit"": {_c.CommissionsTimes - 1} }}
            }} }}";
    }
}

public class WeeklyConfig
{

}
