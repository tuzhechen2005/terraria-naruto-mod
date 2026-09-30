using Terraria;
using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Systems;

// Surface backgrounds for the Naruto regions (M2a spec, "地区背景"): three layers per region in Backgrounds/
// (<Region>Far, <Region>Mid, <Region>Close). A region without its art yet, or with the client setting off, keeps
// vanilla's background.
public static class RegionBackgrounds
{
    public const string Konoha = "Konoha";
    public const string Mist = "Mist";
    public const string Suna = "Suna";
    public const string Snow = "Snow";
    public const string Kusa = "Kusa";
    public const string Myoboku = "Myoboku";

    public static bool Ready(string region) =>
        ShinobiClientConfig.Instance?.RegionBackgrounds != false &&
        ModContent.HasAsset($"ShinobiPrototype/Backgrounds/{region}Far") &&
        ModContent.HasAsset($"ShinobiPrototype/Backgrounds/{region}Mid") &&
        ModContent.HasAsset($"ShinobiPrototype/Backgrounds/{region}Close");

    public static ModSurfaceBackgroundStyle Style(string region) => !Ready(region) ? null : region switch
    {
        Konoha => ModContent.GetInstance<KonohaBackground>(),
        Mist => ModContent.GetInstance<MistBackground>(),
        Suna => ModContent.GetInstance<SunaBackground>(),
        Snow => ModContent.GetInstance<SnowBackground>(),
        Kusa => ModContent.GetInstance<KusaBackground>(),
        Myoboku => ModContent.GetInstance<MyobokuBackground>(),
        _ => null,
    };
}

public abstract class RegionBackground : ModSurfaceBackgroundStyle
{
    protected abstract string Region { get; }

    private int Layer(string layer) =>
        BackgroundTextureLoader.GetBackgroundSlot(Mod, $"Backgrounds/{Region}{layer}");

    // Fade this style in and every other one out, as vanilla does between biomes.
    public override void ModifyFarFades(float[] fades, float transitionSpeed)
    {
        for (int i = 0; i < fades.Length; i++)
            fades[i] = i == Slot ? System.Math.Min(1f, fades[i] + transitionSpeed) : System.Math.Max(0f, fades[i] - transitionSpeed);
    }

    public override int ChooseFarTexture() => Layer("Far");

    public override int ChooseMiddleTexture() => Layer("Mid");

    public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b) => Layer("Close");
}

public sealed class KonohaBackground : RegionBackground { protected override string Region => RegionBackgrounds.Konoha; }
public sealed class MistBackground : RegionBackground { protected override string Region => RegionBackgrounds.Mist; }
public sealed class SunaBackground : RegionBackground { protected override string Region => RegionBackgrounds.Suna; }
public sealed class SnowBackground : RegionBackground { protected override string Region => RegionBackgrounds.Snow; }
public sealed class KusaBackground : RegionBackground { protected override string Region => RegionBackgrounds.Kusa; }
public sealed class MyobokuBackground : RegionBackground { protected override string Region => RegionBackgrounds.Myoboku; }

// The vanilla-biome regions: Wave Country's bridge mist, the desert (Suna), the snow (Land of Snow), the jungle
// (Grass / Waterfall) and the glowing mushrooms (Mount Myoboku), on the surface only.
public abstract class RegionScene : ModSceneEffect
{
    protected abstract string Region { get; }
    protected abstract bool InRegion(Player player);

    // BiomeHigh: vanilla picks the ocean, glowing mushroom and desert backgrounds ahead of any mod style below it
    // (Main.GetPreferredBGStyleForPlayer), so at BiomeMedium the Sand, Myoboku and Mist backgrounds never showed.
    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => RegionBackgrounds.Style(Region);

    public override bool IsSceneEffectActive(Player player) =>
        player.ZoneOverworldHeight && RegionBackgrounds.Ready(Region) && InRegion(player);
}

public sealed class MistScene : RegionScene
{
    protected override string Region => RegionBackgrounds.Mist;
    protected override bool InRegion(Player player) =>
        WaveBridgeWorld.Site.HasValue && WaveBridgeWorld.DistanceToBridgeTiles(player.Center) <= BridgeRules.FogReachTiles;
}

public sealed class SunaScene : RegionScene
{
    protected override string Region => RegionBackgrounds.Suna;
    protected override bool InRegion(Player player) => player.ZoneDesert;
}

public sealed class SnowScene : RegionScene
{
    protected override string Region => RegionBackgrounds.Snow;
    protected override bool InRegion(Player player) => player.ZoneSnow;
}

public sealed class KusaScene : RegionScene
{
    protected override string Region => RegionBackgrounds.Kusa;
    protected override bool InRegion(Player player) => player.ZoneJungle;
}

public sealed class MyobokuScene : RegionScene
{
    protected override string Region => RegionBackgrounds.Myoboku;
    protected override bool InRegion(Player player) => player.ZoneGlowshroom;
}
