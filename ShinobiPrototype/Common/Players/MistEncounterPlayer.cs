using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Common.Players;

// A character's own encounters with the figures in the mist, all triggered on their client:
// the mist preview the first time they walk out near the broken end, and, once they carry three Mist insignia,
// a senbon from the fog that sticks in the deck at their feet (no damage). Each happens once per character in each
// world (a character who saw it in one world sees it again in a new one), remembered by world ID.
public sealed class MistEncounterPlayer : ModPlayer
{
    private const int GlimpseTicks = 120;

    private HashSet<string> previewWorlds = new();
    private HashSet<string> warnedWorlds = new();
    private HashSet<string> forestWorlds = new();

    // Met the boy gathering herbs in the forest (unmasked Haku) in this world.
    public bool MetForestBoy => forestWorlds.Contains(WorldId);

    public void MeetForestBoy() => Remember(forestWorlds, true);

    private static string WorldId => Main.ActiveWorldFileData?.UniqueId.ToString() ?? "";

    public bool SawPreview
    {
        get => previewWorlds.Contains(WorldId);
        private set => Remember(previewWorlds, value);
    }

    public bool Warned
    {
        get => warnedWorlds.Contains(WorldId);
        private set => Remember(warnedWorlds, value);
    }

    private static void Remember(HashSet<string> worlds, bool value)
    {
        if (value)
            worlds.Add(WorldId);
        else
            worlds.Remove(WorldId);
    }

    public override void Initialize()
    {
        previewWorlds = new HashSet<string>();
        warnedWorlds = new HashSet<string>();
        forestWorlds = new HashSet<string>();
    }

    public override void PostUpdate()
    {
        if (Player.whoAmI != Main.myPlayer || Player.dead || MistPreviewSystem.Playing ||
            !WaveBridgeWorld.MistActive || WaveBridgeWorld.Site is not BridgeSite site)
            return;

        int offset = site.OffsetOf((int)(Player.Center.X / 16f));
        int rowsAbove = site.DeckRow(offset) - (int)(Player.Bottom.Y / 16f) + 1;
        bool nearBreak = BridgeRules.NearBrokenEnd(offset, rowsAbove);
        if (!SawPreview)
        {
            // Until the lake ambush the bridge only has fog; the pair in the mist appears after Zabuza's "death".
            if (!nearBreak || !StoryRules.PreviewAllowed(StoryWorld.LakeDone))
                return;
            SawPreview = true;
            MistPreviewSystem.Play();
            return;
        }

        if (BridgeRules.SenbonWarningDue(Warned, SawPreview, WaveBridgeWorld.MistActive,
                Player.CountItem(ModContent.ItemType<MistInsignia>()), nearBreak))
        {
            Warned = true;
            ThrowWarning(site);
        }
    }

    // The thrower stands out on the water past the break; testing away from the bridge (`/m0 senbon`), about
    // fifteen tiles ahead. The needle lands two tiles in front of the player, on the first ground below.
    internal void ThrowWarning(BridgeSite? site)
    {
        bool atBridge = site is BridgeSite bridge && MistSightingSystem.NearBreak(bridge, Player);
        int toward = atBridge ? site!.Value.Dir : Player.direction;
        Vector2 thrower = atBridge
            ? new Vector2(site!.Value.X(BridgeRules.SenbonThrowerOffset) * 16f + 8f, site.Value.WaterY * 16f - 24f)
            : new Vector2(Player.Center.X + toward * 15 * 16f, Player.Bottom.Y - 24f);
        int targetX = (int)(Player.Center.X / 16f) + toward * 2;
        int groundY = (int)(Player.Bottom.Y / 16f);
        while (groundY < (int)(Player.Bottom.Y / 16f) + 12 && !WorldGen.SolidOrSlopedTile(targetX, groundY))
            groundY++;
        Vector2 target = new(targetX * 16f + 8f, groundY * 16f + 4f);
        Vector2 from = thrower + new Vector2(0f, -40f);
        Projectile.NewProjectile(Player.GetSource_Misc("SenbonWarning"), from,
            Vector2.Normalize(target - from) * BridgeRules.SenbonSpeed, ModContent.ProjectileType<SenbonWarning>(),
            0, 0f, Player.whoAmI, target.X, target.Y);
        MistSightingSystem.Glimpse(MistFigure.Haku, thrower, -toward, GlimpseTicks, withWhisper: false);
        MistFigures.Say(thrower, "SenbonWarning");
    }

    public override void SaveData(TagCompound tag)
    {
        if (previewWorlds.Count > 0)
            tag["mistPreviewWorlds"] = previewWorlds.ToList();
        if (warnedWorlds.Count > 0)
            tag["senbonWarnedWorlds"] = warnedWorlds.ToList();
        if (forestWorlds.Count > 0)
            tag["forestBoyWorlds"] = forestWorlds.ToList();
    }

    // Saves from before this was per world stored plain flags; those are dropped, so the scene plays once more.
    public override void LoadData(TagCompound tag)
    {
        previewWorlds = new HashSet<string>(tag.GetList<string>("mistPreviewWorlds"));
        warnedWorlds = new HashSet<string>(tag.GetList<string>("senbonWarnedWorlds"));
        forestWorlds = new HashSet<string>(tag.GetList<string>("forestBoyWorlds"));
    }
}
