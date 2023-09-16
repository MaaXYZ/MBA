namespace MBA.Core.Enums;

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
