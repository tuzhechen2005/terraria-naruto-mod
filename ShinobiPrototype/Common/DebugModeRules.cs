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

    // Terraria's clock: day runs 4:30 AM to 7:30 PM (54000 ticks), night 7:30 PM to 4:30 AM (32400 ticks),
    // at 60 ticks per in-game minute. Accepts day / noon / night / midnight or a 24-hour hh:mm time.
    public static bool TryParseTime(string input, out bool dayTime, out double time)
    {
        dayTime = true;
        time = 0;
        int minutes;
        switch (input.ToLowerInvariant())
        {
            case "day": minutes = 4 * 60 + 30; break;
            case "noon": minutes = 12 * 60; break;
            case "night": minutes = 19 * 60 + 30; break;
            case "midnight": minutes = 0; break;
            default:
                string[] parts = input.Split(':');
                if (parts.Length != 2 || !int.TryParse(parts[0], out int hour) || !int.TryParse(parts[1], out int minute) ||
                    hour is < 0 or > 23 || minute is < 0 or > 59)
                    return false;
                minutes = hour * 60 + minute;
                break;
        }

        int sinceDawn = (minutes - (4 * 60 + 30) + 24 * 60) % (24 * 60);
        dayTime = sinceDawn < 15 * 60;
        time = (dayTime ? sinceDawn : sinceDawn - 15 * 60) * 60.0;
        return true;
    }
}
