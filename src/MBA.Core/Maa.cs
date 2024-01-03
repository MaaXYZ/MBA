using MaaFramework.Binding;

namespace MBA.Core;

public class Maa
{
    public static MaaToolkit Toolkit { get; } = new();
    public static MaaUtility Utility { get; } = new();
}
