using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MBA.Core.Data;

[JsonSerializable(typeof(Config))]
public partial class ConfigContext : JsonSerializerContext
{
    public static void InitOptions()
    {
        s_defaultOptions.WriteIndented = true;
        s_defaultOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        s_defaultOptions.Converters.Add(new JsonStringEnumConverter());
    }
}
