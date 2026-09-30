using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Systems;

// The Hidden Leaf Village as a biome (M2a spec): the village's surface background, and the place every vanilla town
// NPC likes to live.
public sealed class KonohaBiome : ModBiome
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow;

    public override string BestiaryIcon => "ShinobiPrototype/Content/Items/NinjaHandbook";
    public override string BackgroundPath => base.BackgroundPath;

    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle =>
        RegionBackgrounds.Style(RegionBackgrounds.Konoha);

    public override bool IsBiomeActive(Player player) =>
        KonohaWorld.Site is KonohaSite site &&
        Math.Abs(player.Center.X / 16f - site.CenterX) <= KonohaDesign.HalfWidth + 20 &&
        player.Center.Y / 16f > site.GroundY - 150 && player.Center.Y / 16f < site.GroundY + 40;
}

// Everyone likes living in the Leaf, and the Leaf is a planned village: an NPC whose home is inside it is never
// "crowded" (vanilla counts the homes within 25 tiles; the village holds 48 of them), so a full village keeps its
// prices and can still sell pylons.
public sealed class KonohaHappiness : ModSystem
{
    public override void SetStaticDefaults()
    {
        for (int type = 0; type < NPCID.Count; type++)
        {
            if (!NPCID.Sets.ActsLikeTownNPC[type] && !IsVanillaTownNPC(type) || NPCID.Sets.NoTownNPCHappiness[type])
                continue;
            NPCHappiness.Get(type).SetBiomeAffection<KonohaBiome>(AffectionLevel.Like);
        }
    }

    private static bool IsVanillaTownNPC(int type)
    {
        NPC npc = new();
        npc.SetDefaults(type);
        return npc.townNPC;
    }

    public override void Load() => On_ShopHelper.GetNearbyResidentNPCs += NotCrowdedInKonoha;

    private static List<NPC> NotCrowdedInKonoha(On_ShopHelper.orig_GetNearbyResidentNPCs orig, ShopHelper self, NPC npc,
        out int npcsWithinHouse, out int npcsWithinVillage)
    {
        List<NPC> nearby = orig(self, npc, out npcsWithinHouse, out npcsWithinVillage);
        int x = npc.homeless ? (int)(npc.Center.X / 16f) : npc.homeTileX;
        int y = npc.homeless ? (int)(npc.Center.Y / 16f) : npc.homeTileY;
        if (KonohaWorld.Site is KonohaSite site && site.Contains(x, y))
            npcsWithinHouse = Math.Min(npcsWithinHouse, 2);
        return nearby;
    }
}
