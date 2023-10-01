using System.Text.Json.Nodes;
using MBA.Core.Extensions;

namespace MBA.Core.Data;

#pragma warning disable CA1507 // 使用 nameof 表达符号名称

public class DiffTasks
{
    private readonly Config _config;

    public DiffTasks(Config config)
    {
        _config = config;
    }

    public static JsonObject Empty => new();

    #region Daily

    public JsonObject TacticalChallenge =>
        _config.Daily.TacticalChallengeTimes.IsZeroTimes()
        ? TacticalChallenge0
        : TacticalChallenge1;
    private JsonObject TacticalChallenge0 => new()
    {
        ["diff_task"] = new JsonObject
        {
            ["Sub_Start_TacticalChallenge_Partial"] = new JsonObject
            {
                ["times_limit"] = ConfigTimesExtension.ZeroTimes
            },
            ["Sub_End_TacticalChallenge_Partial"] = new JsonObject
            {
                ["times_limit"] = ConfigTimesExtension.ZeroTimes
            },
        }
    };
    private JsonObject TacticalChallenge1 => new()
    {
        ["diff_task"] = new JsonObject
        {
            ["Sub_Start_TacticalChallenge_Partial"] = new JsonObject
            {
                ["times_limit"] = _config.Daily.TacticalChallengeTimes.ClampTimes()
            },
        }
    };

    public JsonObject CommissionsGlobal => new()
    {
        ["enabled"] = _config.Daily.CommissionsAllIn != Enums.Commissions.None,
        ["diff_task"] = new JsonObject
        {
            ["Commissions"] = new JsonObject
            {
                ["next"] = _config.Daily.CommissionsAllIn.ToString()
            },
            ["Click_PlusButton"] = new JsonObject
            {
                ["times_limit"] = ConfigTimesExtension.MaxTimes
            },
            ["Click_MaxButton"] = new JsonObject
            {
                ["enabled"] = true
            },
        }
    };
    public JsonObject BaseDefense => new()
    {
        ["enabled"] = !_config.Daily.CommissionsBaseDefenseTimes.IsZeroTimes(),
        ["diff_task"] = new JsonObject
        {
            ["Click_PlusButton"] = new JsonObject
            {
                ["times_limit"] = _config.Daily.CommissionsBaseDefenseTimes.SubtractOnce()
            },
            ["Click_MaxButton"] = new JsonObject
            {
                ["enabled"] = _config.Daily.CommissionsBaseDefenseTimes.IsMaxTimes()
            },
        }
    };
    public JsonObject ItemRetrieval => new()
    {
        ["enabled"] = !_config.Daily.CommissionsItemRetrievalTimes.IsZeroTimes(),
        ["diff_task"] = new JsonObject
        {
            ["Click_PlusButton"] = new JsonObject
            {
                ["times_limit"] = _config.Daily.CommissionsItemRetrievalTimes.SubtractOnce()
            },
            ["Click_MaxButton"] = new JsonObject
            {
                ["enabled"] = _config.Daily.CommissionsItemRetrievalTimes.IsMaxTimes()
            },
        }
    };

    public JsonObject BountyGlobal => new()
    {
        ["enabled"] = _config.Daily.BountyAllIn != Enums.Bounty.None,
        ["diff_task"] = new JsonObject
        {
            ["BountyGlobal"] = new JsonObject
            {
                ["next"] = _config.Daily.BountyAllIn.ToString()
            },
            ["Click_MaxButton"] = new JsonObject
            {
                ["enabled"] = true
            },
        }
    };
    public JsonObject Overpass => new()
    {
        ["enabled"] = !_config.Daily.BountyOverpassTimes.IsZeroTimes(),
        ["diff_task"] = new JsonObject
        {
            ["Click_PlusButton"] = new JsonObject
            {
                ["times_limit"] = _config.Daily.BountyOverpassTimes.SubtractOnce()
            }
        }
    };
    public JsonObject DesertRailroad => new()
    {
        ["enabled"] = !_config.Daily.BountyDesertRailroadTimes.IsZeroTimes(),
        ["diff_task"] = new JsonObject
        {
            ["Click_PlusButton"] = new JsonObject
            {
                ["times_limit"] = _config.Daily.BountyDesertRailroadTimes.SubtractOnce()
            }
        }
    };
    public JsonObject Classroom => new()
    {
        ["enabled"] = !_config.Daily.BountyClassroomTimes.IsZeroTimes(),
        ["diff_task"] = new JsonObject
        {
            ["Click_PlusButton"] = new JsonObject
            {
                ["times_limit"] = _config.Daily.BountyClassroomTimes.SubtractOnce()
            }
        }
    };

    public JsonObject ScrimmageGlobal => new()
    {
        ["enabled"] = _config.Daily.ScrimmageAllIn != Enums.Scrimmage.None,
        ["diff_task"] = new JsonObject
        {
            ["ScrimmageGlobal"] = new JsonObject
            {
                ["next"] = _config.Daily.ScrimmageAllIn.ToString()
            },
            ["Click_MaxButton"] = new JsonObject
            {
                ["enabled"] = true
            },
        }
    };
    public JsonObject Trinity => new()
    {
        ["enabled"] = !_config.Daily.ScrimmageTrinityTimes.IsZeroTimes(),
        ["diff_task"] = new JsonObject
        {
            ["Click_PlusButton"] = new JsonObject
            {
                ["times_limit"] = _config.Daily.ScrimmageTrinityTimes.SubtractOnce()
            }
        }
    };
    public JsonObject Gehenna => new()
    {
        ["enabled"] = !_config.Daily.ScrimmageGehennaTimes.IsZeroTimes(),
        ["diff_task"] = new JsonObject
        {
            ["Click_PlusButton"] = new JsonObject
            {
                ["times_limit"] = _config.Daily.ScrimmageGehennaTimes.SubtractOnce()
            }
        }
    };
    public JsonObject Millennium => new()
    {
        ["enabled"] = !_config.Daily.ScrimmageMillenniumTimes.IsZeroTimes(),
        ["diff_task"] = new JsonObject
        {
            ["Click_PlusButton"] = new JsonObject
            {
                ["times_limit"] = _config.Daily.ScrimmageMillenniumTimes.SubtractOnce()
            }
        }
    };

    #endregion
}
