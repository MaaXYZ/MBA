using MaaToolKit.Extensions.ComponentModel;
using MaaToolKit.Extensions.Enums;
using MBA.Core.Data;
using MBA.Core.Enums;
using MBA.Core.Managers;

namespace MBA.Core;

public class Main
{
    private static Serilog.ILogger Log => LogManager.Logger;

    public static Main Current { get; } = new();

    // TODO: 去除静态
    private static Config Config { get; } = ConfigManager.Config;

    static Main()
    {
        MaaObject.FrameworkLogDir = GlobalInfo.DebugFullPath;
        MaaObject.DebugMode = Config.UI.DebugMode;
    }

    public void Start()
    {
        var location = $"{nameof(Main)}.{nameof(Start)}";

        TaskManager.RunTask(() =>
        {
            var maa = GetMaa();
            var tasks = GetTasks();
            if (maa.Instance.Initialized)
                RunTasks(maa, tasks);
            else
                Log.Error("Failed to init Maa instance, a connection error or resource file corruption occurred, please refer to the log.");
        },
        location);
    }

    private MaaObject GetMaa()
    {
        var maa = new MaaObject(
                Config.Core.Adb,
                Config.Core.AdbAddress,
                Config.Core.ControlType,
                ConfigManager.AdbConfig,
                GlobalInfo.BaseResourceFullPath,
                $"{GlobalInfo.ResourceFullPath}/{Config.Game.Language}");

        maa.Controller.SetOption(ControllerOption.ScreenshotTargetShortSide, GlobalInfo.ScreenshotHeight);

        return maa;
    }

    private IList<TaskType> GetTasks()
    {
        var daily = TaskType.Daily;
        var dailyTasks = new List<TaskType>(daily.ToList());
        var configTasks = new List<TaskType>(Config.Tasks);

        int i = configTasks.IndexOf(daily);
        if (i >= 0)
        {
            configTasks.RemoveAll(x => x == daily);
            dailyTasks.RemoveAll(configTasks.Contains);
            configTasks.InsertRange(i, dailyTasks);
        }

        configTasks.RemoveAll(Config.TasksExcept.Contains);
        return configTasks;
    }

    private bool RunTasks(MaaObject maa, IList<TaskType> tasks)
    {
        bool hasFailed = false;
        Log.Information("Task List: {list}.", tasks);

        foreach (TaskType task in tasks)
        {
            // Fix: MaaFramework PackageEntry
            var diff = $@"{{ ""diff_task"": {{
                ""Sub_StartApp"": {{ ""package"": ""{Config.Game.PackageEntry}"" }}
                }} }}";
            maa.Instance
               .AppendTask(TaskType.StartUp.ToString(), diff);

            Log.Information("{task} start.", task);

            diff = task switch
            {
                TaskType.Commissions => Config.Daily.DiffTask.Commissions,
                TaskType.TacticalChallenge => Config.Daily.DiffTask.TacticalChallenge,
                _ => "{}",
            };
            var status = maa.Instance
                            .AppendTask(task.ToString(), diff)
                            .Wait();

            // TODO: MaaJob 和 MaaJobStatus 包含 任务名 及其参数

            if (status == MaaJobStatus.Success)
            {
                Log.Information("{task} done. Result: {status}", task, status);
            }
            else
            {
                Log.Warning("{task} done. Result: {status}", task, status);
                hasFailed = true;
            }
        }

        return hasFailed;
    }
}
