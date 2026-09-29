using ShinobiPrototype.Common;

static void Check(bool valid, bool actual, bool expectedValid, bool expected, string name)
{
    if (valid != expectedValid || actual != expected)
        throw new Exception($"{name}: expected ({expectedValid}, {expected}), got ({valid}, {actual})");
    Console.WriteLine($"PASS {name}");
}

bool valid = DebugModeRules.TryResolveGodMode(false, null, out bool enabled);
Check(valid, enabled, true, true, "Toggle on");
valid = DebugModeRules.TryResolveGodMode(true, null, out enabled);
Check(valid, enabled, true, false, "Toggle off");
valid = DebugModeRules.TryResolveGodMode(false, "ON", out enabled);
Check(valid, enabled, true, true, "Explicit on is case insensitive");
valid = DebugModeRules.TryResolveGodMode(true, "off", out enabled);
Check(valid, enabled, true, false, "Explicit off");
valid = DebugModeRules.TryResolveGodMode(true, "unknown", out enabled);
Check(valid, enabled, false, true, "Invalid option preserves current state");

static void CheckTime(string input, bool expectedDay, double expectedTime)
{
    if (!DebugModeRules.TryParseTime(input, out bool day, out double time) || day != expectedDay || time != expectedTime)
        throw new Exception($"Time {input}: expected ({expectedDay}, {expectedTime}), got ({day}, {time})");
    Console.WriteLine($"PASS Time {input}");
}

CheckTime("day", true, 0);
CheckTime("noon", true, 27000);
CheckTime("night", false, 0);
CheckTime("midnight", false, 16200);
CheckTime("19:29", true, 53940);
CheckTime("04:29", false, 32340);
if (DebugModeRules.TryParseTime("25:00", out _, out _) || DebugModeRules.TryParseTime("dusk", out _, out _))
    throw new Exception("Invalid times must be rejected");
Console.WriteLine("PASS Invalid times rejected");
