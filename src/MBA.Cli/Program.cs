using MBA.Core.Data;
using MBA.Core.Enums;
using MBA.Core.Managers;

namespace MBA.Cli;

internal class Program
{
    static Config Config => ConfigManager.Config;

    static Serilog.ILogger Log => LogManager.Logger;

    static Program()
    {
        GlobalInfo.IsCli = true;
    }

    static int Main(string[] args)
    {
        TipChangeLog();
        Console.WriteLine(Greeting);

        if (!ResetConfig())
        {
            Log.Fatal("Failed to reset config.");
            Console.WriteLine("请检查 配置输入 是否正确！");
            TipPause();
            return -1;
        }

        if (!ProcArgs(args))
        {
            Log.Fatal("Failed to parse args.");
            Console.WriteLine("请检查 命令行参数 是否正确！");
            TipPause();
            return -1;
        }

        if (!Config.Tasks.Any())
        {
            Log.Fatal("Task List is empty.");
            Console.WriteLine("请检查 任务列表 是否为空！");
            TipPause();
            return -1;
        }

        Core.Main.Current.Start();

        TipNewVersion();
        TipPause();
        return 0;
    }

    private static void TipChangeLog()
    {
        if (!VersionManager.Updated)
            return;

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\t\t\t    ChangLog");
        Console.WriteLine(VersionManager.ChangeLog);
        Console.ForegroundColor = ConsoleColor.White;
    }

    private static void TipNewVersion()
    {
        if (!VersionManager.Released)
            return;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("New version released, available for download at https://github.com/MaaAssistantArknights/MBA/releases/latest.");
        Console.ForegroundColor = ConsoleColor.White;
    }

    private static void TipPause()
    {
        if (!s_pasue)
            return;

        Console.WriteLine("按任意键关闭此窗口. . .");
        Console.ReadKey();
    }

    static string Greeting =>
$@"
    MBA.Cli.exe [arg] ...
        arg:
            设备地址:端口号（e.g. 127.0.0.1:5555）
            任务名（e.g. Tasks）
            任务Id（e.g. 1）
            完成后不暂停（NoPause）
    也可以修改 config.json 来进行相关配置（命令行参数优先于 config.json）

    通过命令行执行的任务是一次性的，将不会保存于 config.json

    进行配置时可直接按回车键使用默认值 / 原有配置

    {(Config.UI.FirstStartUp
        ? "第一次启动请进行相关配置以生成 config.json 文件"
        : $"{WaitTime} 秒内按下任意按键重新进行相关配置")}
";

    const int WaitTime = 3;
    const string NoPause = "nopause";
    private static bool s_pasue = true;

    /// <summary>
    ///     启动时重设配置
    /// </summary>
    /// <returns></returns>
    static bool ResetConfig()
    {
        DateTime beginWait = DateTime.Now;
        while (!Config.UI.FirstStartUp
            && !Console.KeyAvailable
            && DateTime.Now.Subtract(beginWait).TotalSeconds < WaitTime)
        {
            Thread.Sleep(250);
        }

        if (Console.KeyAvailable || Config.UI.FirstStartUp)
        {
            if (Console.KeyAvailable)
                _ = Console.ReadKey();

            if (!SetConfig())
                return false;

            Console.WriteLine();
            Config.UI.FirstStartUp = false;
        }

        ConfigManager.SaveConfig();
        return true;
    }

    /// <summary>
    ///     启动时解析命令行参数
    /// </summary>
    /// <param name="args"></param>
    /// <returns>成功与否</returns>
    static bool ProcArgs(string[] args)
    {
        var taskList = new List<TaskType>();
        foreach (string arg in args)
        {
            if (string.IsNullOrWhiteSpace(arg)) continue;
            var value = arg;

            if (Enum.TryParse(value, out TaskType task))
            {
                taskList.Add(task);
                continue;
            }

            if (value.ToLower() == NoPause)
            {
                s_pasue = false;
                continue;
            }

            if (CheckAdbAddress(value, out var address))
            {
                Config.Core.AdbAddress = address;
                continue;
            }

            Log.Error("未知任务名/Id：{arg} (解析为：{value})", arg, value);
            return false;
        }

        if (taskList.Contains(TaskType.Test))
            Log.Warning("解析到测试用任务, 请确保测试任务存在!");
        if (taskList.Any())
            Config.Tasks = taskList;

        return true;
    }

    /// <summary>
    ///     设置配置
    /// </summary>
    /// <returns>成功与否</returns>
    static bool SetConfig()
    {
        var ret = true;
        Console.WriteLine();
        ret &= SetTasks();
        Console.WriteLine();
        ret &= SetAdb();
        Console.WriteLine();
        ret &= SetAdbAddress();
        do
        {
            Console.WriteLine();
            _ = SetGameLanguage();
            Console.WriteLine();
            _ = SetGameServer();
        }
        while (!Config.Game.LanguageServer.IsValid());
        Console.WriteLine();
        return ret;
    }

    /// <summary>
    ///     设置 Adb 配置
    /// </summary>
    /// <returns>成功与否</returns>
    static bool SetAdb()
    {
        Console.Write($"请输入 {DocToConsole(Config.Document.Adb)}\n: ");
        string? read = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(read))
        {
            Console.WriteLine($"使用默认路径：{Config.Core.Adb}");
            return true;
        }

        var ret = CheckAdbPath(read, out var path);
        Config.Core.Adb = path;
        return ret;
    }

    /// <summary>
    ///     设置连接地址配置
    /// </summary>
    /// <returns>成功与否</returns>
    static bool SetAdbAddress()
    {
        Console.Write($"请输入 {DocToConsole(Config.Document.AdbAddress)}\n:");
        string? read = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(read))
        {
            Console.WriteLine($"使用默认地址：{Config.Core.AdbAddress}");
            return true;
        }

        var ret = CheckAdbAddress(read, out var address);
        Config.Core.AdbAddress = address;
        return ret;
    }

    /// <summary>
    ///     设置游戏语言配置
    /// </summary>
    /// <returns>成功与否</returns>
    static bool SetGameLanguage()
    {
        Console.Write($"请输入 {DocToConsole(Config.Document.Language)}\n:");
        string? read = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(read))
        {
            Console.WriteLine($"使用默认语言：{Config.Game.Language}");
            return true;
        }

        var ret = Enum.TryParse(read, out GameLanguageServer type);
        Config.Game.Language = type;
        return ret;
    }

    /// <summary>
    ///     设置游戏服务器配置
    /// </summary>
    /// <returns>成功与否</returns>
    static bool SetGameServer()
    {
        Console.Write($"请输入 {DocToConsole(Config.Document.Server)}\n: ");
        string? read = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(read))
        {
            Console.WriteLine($"使用默认服务器：{Config.Game.Server}");
            return true;
        }

        var ret = Enum.TryParse(read, out GameLanguageServer type);
        Config.Game.Server = type;
        return ret;
    }

    /// <summary>
    ///     设置执行任务配置
    /// </summary>
    /// <returns>成功与否</returns>
    static bool SetTasks()
    {
        Console.Write($"请输入 {DocToConsole(Config.Document.Tasks)}\n可自定义顺序，以空格分隔，例如 1 2 3\n: ");
        string? read = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(read))
        {
            Console.WriteLine($"使用默认任务列表：[{string.Join(", ", Config.Tasks)}]");
            return true;
        }

        var taskIds = read.Split(' ', ',', '"');
        return ProcArgs(taskIds);
    }

    static string DocToConsole(string value)
        => value.Replace(", ", Environment.NewLine + '\t').Replace("：", Environment.NewLine + '\t');

    static bool CheckAdbAddress(string value, out string result)
    {
        result = value.Replace('：', ':').Replace('。', '.');
        var parts = result.Split(':');
        if (parts.Length != 2)
        {
            var count = MaaToolKit.Extensions.MaaTool.FindDevice(Config.Core.Adb);
            for (ulong i = 0; i < count; i++)
            {
                if (MaaToolKit.Extensions.MaaTool.GetDeviceAdbSerial(i) == value)
                    return true;
            }
            return false;
        }
        if (!int.TryParse(parts[1], out int port)) return false;
        if (port < 0 || port > 65535) return false;
        try
        {
            return System.Net.IPAddress.TryParse(parts[0], out _);
        }
        catch (Exception)
        {
            return false;
        }
    }

    static bool CheckAdbPath(string value, out string result)
    {
        result = value;
        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = result,
                Arguments = "version",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        try
        {
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output.Contains("Android Debug Bridge version");
        }
        catch (Exception)
        {
            return false;
        }
    }

    // TODO: 多配置
    static bool CheckConfigJsonFile(string value, out string result)
    {
        result = value;
        if (File.Exists(result)) return true;
        result = Path.GetFileNameWithoutExtension(value);
        result = Path.Combine("config", $"{result}.json");
        return File.Exists(result);
    }
}
