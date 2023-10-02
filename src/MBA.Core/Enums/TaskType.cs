using MBA.Core.Data;
using MBA.Core.Managers;

namespace MBA.Core.Enums;

/* 新增一个 TaskType 可能需要改动：
 * 1. this 添加 TaskType
 * 2. out  添加至 pipeline json 中
 * 3. this 添加 private static readonly List<TaskType>
 * 4. this 添加至 ToList
 * 5. this 添加至 TryRunTasks 中的 Replace
 * 6. out  添加至 DiffTasks
 * 7. this 添加至 EnableTask 中的 switch
 */

public enum TaskType
{
    Bounty = 1,
    Cafe = 2,
    Club = 3,
    Commissions = 4,
    //Crafting = 5,
    Mailbox = 6,
    Scrimmage = 7,
    //Shop = 8,
    TacticalChallenge = 9,
    Tasks = 10,

    StartUp = 100,
    Daily = 101,
    Weekly = 102,

    // 以下任务不对用户公开，以保持可配置任务的统一
    Test = 0,

    BountyGlobal = -1_0,
    Overpass = -1_1,
    DesertRailroad = -1_2,
    Classroom = -1_3,
    BountyChinese = -1_9,

    CommissionsGlobal = -4_0,
    BaseDefense = -4_1,
    ItemRetrieval = -4_2,

    ScrimmageGlobal = -7_0,
    Trinity = -7_1,
    Gehenna = -7_2,
    Millennium = -7_3,
    ScrimmageChinese = -7_9,

}

public static class TaskTypeExtensions
{
    private static Serilog.ILogger Log => LogManager.Logger;

    private static readonly List<TaskType> _dailyTasks = new()
    {
        TaskType.StartUp,
        //TaskType.Crafting,          TODO
        TaskType.TacticalChallenge,
        TaskType.Bounty,
        TaskType.Club,
        TaskType.Mailbox,
        //TaskType.Shop,              TODO
        TaskType.Cafe,
        TaskType.Tasks,
        TaskType.Commissions,
        TaskType.Scrimmage,
        TaskType.Tasks,
    };

    private static readonly List<TaskType> _commissionsTasks = new()
    {
        TaskType.BaseDefense,
        TaskType.ItemRetrieval,
        TaskType.CommissionsGlobal
    };

    private static readonly List<TaskType> _bountyGlobalTasks = new()
    {
        TaskType.Overpass,
        TaskType.DesertRailroad,
        TaskType.Classroom,
        TaskType.BountyGlobal,
    };

    private static readonly List<TaskType> _bountyChineseTasks = new()
    {
        TaskType.BountyChinese
    };

    private static readonly List<TaskType> _scrimmageGlobalTasks = new()
    {
        TaskType.Trinity,
        TaskType.Gehenna,
        TaskType.Millennium,
        TaskType.ScrimmageGlobal,
    };

    private static readonly List<TaskType> _scrimmageChineseTasks = new()
    {
        // TaskType.ScrimmageChinese
    };

    public static List<TaskType> ToList(this TaskType task, GameLanguageServer server) => task switch
    {
        TaskType.Daily => _dailyTasks,
        TaskType.Commissions => _commissionsTasks,
        TaskType.Bounty when server.IsChinese() => _bountyChineseTasks,
        TaskType.Bounty => _bountyGlobalTasks,
        TaskType.Scrimmage when server.IsChinese() => _scrimmageChineseTasks,
        TaskType.Scrimmage => _scrimmageGlobalTasks,
        _ => new List<TaskType> { task },
    };

    public static void Replace(this List<TaskType> tasks, GameLanguageServer server)
    {
        tasks.Replace(TaskType.Bounty, server)
             .Replace(TaskType.Commissions, server)
             .Replace(TaskType.Scrimmage, server);
    }

    public static bool Enabled(this TaskType task, DiffTasks diffTasks, out string diffTask)
    {
        var jsonObject = task switch
        {
            TaskType.TacticalChallenge => diffTasks.TacticalChallenge,

            TaskType.CommissionsGlobal => diffTasks.CommissionsGlobal,
            TaskType.BaseDefense => diffTasks.BaseDefense,
            TaskType.ItemRetrieval => diffTasks.ItemRetrieval,

            TaskType.BountyGlobal => diffTasks.BountyGlobal,
            TaskType.Overpass => diffTasks.Overpass,
            TaskType.DesertRailroad => diffTasks.DesertRailroad,
            TaskType.Classroom => diffTasks.Classroom,

            TaskType.ScrimmageGlobal => diffTasks.ScrimmageGlobal,
            TaskType.Trinity => diffTasks.Trinity,
            TaskType.Gehenna => diffTasks.Gehenna,
            TaskType.Millennium => diffTasks.Millennium,

            _ => DiffTasks.Empty,
        };

        diffTask = jsonObject.ToJsonString();
        Log.Debug("{task} Diff Task: {diff}.", task, diffTask);

        return (bool)(jsonObject["enabled"] ?? true);
    }

    public static List<TaskType> Replace(this List<TaskType> tasks, TaskType type, GameLanguageServer server)
    {
        int i = tasks.IndexOf(type);
        if (i >= 0)
        {
            tasks.RemoveAt(i);
            tasks.Replace(type, server);
            tasks.InsertRange(i, type.ToList(server));
        }

        return tasks;
    }
}
