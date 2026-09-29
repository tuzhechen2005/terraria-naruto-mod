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
