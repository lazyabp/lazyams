using System.Text.Json;

namespace Lazy.Core.Json;

public class UpperCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return name.Substring(0, 1).ToUpperInvariant() + name.Substring(1);
    }
}
