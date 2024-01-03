using MaaFramework.Binding;
using MBA.Core.Data;
using MBA.Core.Enums;
using MBA.Core.Managers;

namespace MBA.Core;

public class Sensei
{
    private static Serilog.ILogger Log => LogManager.Logger;

    public static Sensei Agent { get; } = new();

    // TODO: 去除静态
    private static Config Config { get; } = ConfigManager.Config;

    public void Start()
    {
        var location = $"{nameof(Sensei)}.{nameof(Start)}";

        TaskManager.RunTask(() =>
        {
            // remark: lock config
            ConfigManager.LogConfig();

            using var maa = GetMaa();
            var tasks = GetTasks();
            if (!maa.Initialized)
                Log.Error("Failed to init Maa instance, a connection error or resource file corruption occurred, please refer to the log.");

            if (TryRunTasks(maa, tasks))
                Log.Information("Congratulations! All tasks have been successful!");
            else
                Log.Warning("Unfortunately, some tasks have failed...");
        },
        location);
    }

    private MaaInstance GetMaa()
    {
        var maa = new MaaInstance
        {
            Controller = new MaaAdbController(Config.Core.Adb, Config.Core.AdbAddress, Config.Core.ControlType, ConfigManager.AdbConfig, "./MaaAgentBinary"),
            Resource = new MaaResource(GlobalInfo.BaseResourceFullPath, $"{GlobalInfo.ResourceFullPath}/{Config.Game.Language}"),
            DisposeOptions = DisposeOptions.All,
        };

        maa.Controller.SetOption(
            ControllerOption.DefaultAppPackageEntry,
            Config.Game.LanguageServer.GetPackageEntry());

        return maa;
    }

    private List<TaskType> GetTasks()
    {
        var configTasks = new List<TaskType>(Config.Tasks);
        configTasks.Replace(TaskType.Daily, Config.Game.Server);
        configTasks.RemoveAll(Config.TasksExcept.Contains);
        return configTasks;
    }

    private bool TryRunTasks(MaaInstance maa, List<TaskType> tasks)
    {
        Log.Information("Task List: {list}.", tasks);
        tasks.Replace(Config.Game.Server);
        Log.Debug("Entry Task List: {list}.", tasks);

        var success = true;
        var failedTasks = new List<TaskType>(tasks.Count);
        var diffTasks = new DiffTasks(Config);

        foreach (TaskType task in tasks)
        {
            if (task.Enabled(diffTasks, out var diffTask))
            {
                Log.Information("{task} Started.", task);
            }
            else
            {
                Log.Debug("{task} Skipped.", task);
                continue;
            }

            var status = maa.AppendTask(task.ToString(), diffTask)
                            .Wait();

            // TODO: MaaJob 和 MaaJobStatus 包含 任务名 及其参数

            if (status == MaaJobStatus.Success)
            {
                Log.Information("{task} Completed. Result: {status}", task, status);
            }
            else
            {
                success = false;
                failedTasks.Add(task);
                Log.Error("{task} Completed. Result: {status}", task, status);
            }
        }

        if (failedTasks.Any())
        {
            Log.Warning("Failed task List: {failedTasks}", failedTasks);
        }
        return success;
    }
}
