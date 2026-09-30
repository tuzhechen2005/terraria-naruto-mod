using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items.StyleCores;

namespace ShinobiPrototype.Common.Players;

// Which style core the player wears this tick, and its technique on the style key (cooldown, chakra).
public sealed class StyleCorePlayer : ModPlayer
{
    public StyleCore Worn { get; set; }
    public int Cooldown { get; private set; }

    public StyleSchool School => Worn?.School ?? StyleSchool.None;

    public override void ResetEffects() => Worn = null;

    public override void PostUpdate()
    {
        if (Cooldown > 0)
            Cooldown--;
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (ShinobiKeybinds.StyleTechnique?.JustPressed != true)
            return;
        if (Worn is not StyleCore core)
        {
            Main.NewText("未装备流派核心。", 200, 200, 200);
            return;
        }
        ChakraPlayer chakra = Player.GetModPlayer<ChakraPlayer>();
        if (!StyleCoreRules.CanUseTechnique(core.School, Cooldown, chakra.Chakra, core.TechniqueCost))
        {
            Main.NewText(Cooldown > 0 ? "流派奥义冷却中。" : "查克拉不足。", 200, 200, 200);
            return;
        }
        if (!chakra.TrySpend(core.TechniqueCost))
            return;
        Cooldown = core.TechniqueCooldown;
        core.UseTechnique(Player);
    }
}
