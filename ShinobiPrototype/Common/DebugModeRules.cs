#nullable enable

namespace ShinobiPrototype.Common;

public static class DebugModeRules
{
    public static bool TryResolveGodMode(bool current, string? option, out bool enabled)
    {
        enabled = current;
        if (option is null)
        {
            enabled = !current;
            return true;
        }

        if (option.Equals("on", System.StringComparison.OrdinalIgnoreCase))
        {
            enabled = true;
            return true;
        }

        if (option.Equals("off", System.StringComparison.OrdinalIgnoreCase))
        {
            enabled = false;
            return true;
        }

        return false;
    }
}
