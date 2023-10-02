namespace MBA.Core.Extensions;

internal static class ConfigTimesExtension
{
    public static int ZeroTimes => 0;

    public static int MaxTimes => 99;

    public static int SubtractOnce(this int times)
        => (times - 1).ClampTimes();

    public static int ClampTimes(this int times)
        => int.Clamp(times, ZeroTimes, MaxTimes);

    public static bool IsMaxTimes(this int times)
        => times >= MaxTimes;

    public static bool IsZeroTimes(this int times)
        => times <= ZeroTimes;

}
