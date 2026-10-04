using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Systems;

public sealed class ShinobiKeybinds : ModSystem
{
    public static ModKeybind Substitution { get; private set; }
    // The worn style core's technique (specs/流派系统.spec.md; default key awaiting the user's decision).
    public static ModKeybind StyleTechnique { get; private set; }
    // Hand seals (specs/装备与忍术系统.spec.md): one key per seal slot; a tap forms that many seals and casts (user,
    // 2026-10-03: holding one key and letting go at the right count was impossible mid-fight, and held it off WASD).
    public static ModKeybind Seal2 { get; private set; }
    public static ModKeybind Seal4 { get; private set; }
    public static ModKeybind Seal6 { get; private set; }

    public override void Load()
    {
        Substitution = KeybindLoader.RegisterKeybind(Mod, "Substitution", "F");
        StyleTechnique = KeybindLoader.RegisterKeybind(Mod, "StyleTechnique", "V");
        Seal2 = KeybindLoader.RegisterKeybind(Mod, "Seal2", "Z");
        Seal4 = KeybindLoader.RegisterKeybind(Mod, "Seal4", "X");
        Seal6 = KeybindLoader.RegisterKeybind(Mod, "Seal6", "C");
    }

    public override void Unload()
    {
        Substitution = null;
        StyleTechnique = null;
        Seal2 = Seal4 = Seal6 = null;
    }

    public static ModKeybind SealKey(int seals) => seals switch { 2 => Seal2, 4 => Seal4, _ => Seal6 };

    public static string SealKeyName(int seals)
    {
        var keys = SealKey(seals)?.GetAssignedKeys();
        return keys is { Count: > 0 } ? keys[0] : Loc.Get("Keys.Unbound");
    }

    // Name of the key currently bound to the jutsu, for hints and the handbook.
    public static string SubstitutionKeyName()
    {
        var keys = Substitution?.GetAssignedKeys();
        return keys is { Count: > 0 } ? keys[0] : Loc.Get("Keys.UnboundSet");
    }
}
