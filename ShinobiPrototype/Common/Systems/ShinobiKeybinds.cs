using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Systems;

public sealed class ShinobiKeybinds : ModSystem
{
    public static ModKeybind Substitution { get; private set; }
    // The worn style core's technique (specs/流派系统.spec.md; default key awaiting the user's decision).
    public static ModKeybind StyleTechnique { get; private set; }
    // Hand seals (specs/装备与忍术系统.spec.md): held to form seals, let go to cast.
    public static ModKeybind Seal { get; private set; }

    public override void Load()
    {
        Substitution = KeybindLoader.RegisterKeybind(Mod, "Substitution", "F");
        StyleTechnique = KeybindLoader.RegisterKeybind(Mod, "StyleTechnique", "V");
        Seal = KeybindLoader.RegisterKeybind(Mod, "Seal", "C");
    }

    public override void Unload()
    {
        Substitution = null;
        StyleTechnique = null;
        Seal = null;
    }

    public static string SealKeyName()
    {
        var keys = Seal?.GetAssignedKeys();
        return keys is { Count: > 0 } ? keys[0] : "未绑定（请在按键设置中设置）";
    }

    // Name of the key currently bound to the jutsu, for hints and the handbook.
    public static string SubstitutionKeyName()
    {
        var keys = Substitution?.GetAssignedKeys();
        return keys is { Count: > 0 } ? keys[0] : "未绑定（请在按键设置中设置）";
    }
}
