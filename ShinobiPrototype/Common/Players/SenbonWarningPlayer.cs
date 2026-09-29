using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Common.Players;

// Once a player carries three Mist insignia and walks out to the broken end, a senbon flies out of the mist and
// sticks in the deck at their feet (no damage), thrown by a masked shape glimpsed in the fog. Once per character.
public sealed class SenbonWarningPlayer : ModPlayer
{
    private const int GlimpseTicks = 120;

    public bool Warned { get; private set; }

    public override void Initialize() => Warned = false;

    public override void PostUpdate()
    {
        if (Player.whoAmI != Main.myPlayer || Warned || Player.dead || MistPreviewSystem.Playing ||
            WaveBridgeWorld.Site is not BridgeSite site)
            return;

        int offset = site.OffsetOf((int)(Player.Center.X / 16f));
        int rowsAbove = site.DeckRow(offset) - (int)(Player.Bottom.Y / 16f) + 1;
        if (!BridgeRules.SenbonWarningDue(Warned, WaveBridgeWorld.PreviewDone, WaveBridgeWorld.MistActive,
                Player.CountItem(ModContent.ItemType<MistInsignia>()), BridgeRules.NearBrokenEnd(offset, rowsAbove)))
            return;

        Warned = true;
        int towardSea = site.Dir;
        Vector2 thrower = new(site.X(BridgeRules.SenbonThrowerOffset) * 16f + 8f, site.WaterY * 16f - 24f);
        int targetX = (int)(Player.Center.X / 16f) + towardSea * 2;
        Vector2 target = new(targetX * 16f + 8f, site.DeckRow(site.OffsetOf(targetX)) * 16f + 4f);
        Vector2 from = thrower + new Vector2(0f, -40f);
        Projectile.NewProjectile(Player.GetSource_Misc("SenbonWarning"), from,
            Vector2.Normalize(target - from) * BridgeRules.SenbonSpeed, ModContent.ProjectileType<SenbonWarning>(),
            0, 0f, Player.whoAmI, target.X, target.Y);
        MistSightingSystem.Glimpse(MistFigure.Haku, thrower, -site.Dir, GlimpseTicks, withWhisper: false);
        MistFigures.Say(thrower, "SenbonWarning");
    }

    public override void SaveData(TagCompound tag)
    {
        if (Warned)
            tag["senbonWarned"] = true;
    }

    public override void LoadData(TagCompound tag) => Warned = tag.GetBool("senbonWarned");
}
