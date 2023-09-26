using System.Text;
using MBA.Core.Managers;
using G = MBA.Core.Enums.GameLanguageServer;

namespace MBA.Core.Enums;

[Flags]
public enum GameLanguageServer
{
    None = 0,

    // A NonMask is a mask that not contains the Server and its Languages
    NonJPMask = ~(Japanese | JP),
    NonGLMask = ~(Global | KR | EN | TH | TC),
    NonCNMask = ~(Chinese | SC),
    LanguageMask = 0xFF,
    ServerMask = 0xFFFF << 8,

    // Language
    JP = 0x01,
    KR = 0x02,
    EN = 0x04,
    TH = 0x08,
    TC = 0x10,
    SC = 0x20,

    // Server
    Japanese
        = 0x01 << 8,

    Global
        = 0xFE << 8,
    Korea
        = 0x02 << 8,
    TaiwanHongKongMacao
        = 0x04 << 8,
    NorthAmerica
        = 0x08 << 8,
    Europe
        = 0x10 << 8,
    Asia
        = 0x20 << 8,

    Chinese
        = 0xF << 16,
    YoStarCN
        = 0x1 << 16,
    Bilibili
        = 0x2 << 16,
    Ourplay
        = 0x4 << 16,
    OthersCN
        = 0x8 << 16,
}

public static class GameLanguageServerExtensions
{
    private static readonly string _packageEntryJapanese = Encoding.UTF8.GetString(Convert.FromBase64String("Y29tLllvc3RhckpQLkJsdWVBcmNoaXZlL2NvbS55b3N0YXJqcC5ibHVlYXJjaGl2ZS5NeFVuaXR5UGxheWVyQWN0aXZpdHk="));
    private static readonly string _packageEntryGlobal = Encoding.UTF8.GetString(Convert.FromBase64String("Y29tLm5leG9uLmJsdWVhcmNoaXZlL2NvbS5uZXhvbi5ibHVlYXJjaGl2ZS5NeFVuaXR5UGxheWVyQWN0aXZpdHk="));
    private static readonly string _packageEntryYoStarCN = Encoding.UTF8.GetString(Convert.FromBase64String("Y29tLlJvYW1pbmdTdGFyLkJsdWVBcmNoaXZlL2NvbS55b3N0YXIuc3VwZXJzZGsuYWN0aXZpdHkuWW9TdGFyU3BsYXNoQWN0aXZpdHk="));
    private static readonly string _packageEntryBilibili = Encoding.UTF8.GetString(Convert.FromBase64String("Y29tLlJvYW1pbmdTdGFyLkJsdWVBcmNoaXZlLmJpbGliaWxpL2NvbS55b3N0YXIuc3VwZXJzZGsuYWN0aXZpdHkuWW9TdGFyU3BsYXNoQWN0aXZpdHk="));

    public static string GetPackageEntry(this G type)
        => type.IsValidJP() ? _packageEntryJapanese
         : type.IsValidGL() ? _packageEntryGlobal
        : type.IsYoStarCN() ? _packageEntryYoStarCN
        : type.IsBilibili() ? _packageEntryBilibili
        : ConfigManager.Config.Game.PackageEntry;

    public static G GetLanguage(this G type)
        => type & G.LanguageMask;

    public static G GetServer(this G type)
        => type & G.ServerMask;

    public static bool IsValid(this G type)
        => type.IsValidJP() || type.IsValidGL() || type.IsValidCN();

    internal static bool IsValidJP(this G type)
        => (type & G.NonJPMask) == G.None;

    internal static bool IsValidGL(this G type)
        => (type & G.NonGLMask) == G.None;

    internal static bool IsValidCN(this G type)
        => (type & G.NonCNMask) == G.None;

    internal static bool IsYoStarCN(this G type)
        => type.IsValidCN() && ((type & G.YoStarCN) == G.YoStarCN);

    internal static bool IsBilibili(this G type)
        => type.IsValidCN() && ((type & G.Bilibili) == G.Bilibili);
}
