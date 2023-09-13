namespace MBA.Core.Enums;

public enum TaskType
{
    Bounty = 1,
    Cafe,
    Club,
    Commissions,
    Crafting,
    Mailbox,
    Scrimmage,
    Shop,
    TacticalChallenge,
    Tasks,

    Daily,
    Weekly,
    StartUp,
    Test = 0,
}

public static class TaskTypeExtensions
{
    private static readonly List<TaskType> _taskList = new()
    {
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

    public static IList<TaskType> ToList(this TaskType task) => task switch
    {
        TaskType.Daily => _taskList,
        _ => new List<TaskType> { task },
    };
}
