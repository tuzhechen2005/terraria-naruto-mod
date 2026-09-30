using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
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
    // Ground rows of the close layers, read from the textures once (BackgroundLayoutRules.GroundRow).
    private static readonly Dictionary<int, int> groundRows = new();

    protected abstract string Region { get; }

    public override void Unload() => groundRows.Clear();

    private int Layer(string layer)
    {
        int slot = BackgroundTextureLoader.GetBackgroundSlot(Mod, $"Backgrounds/{Region}{layer}");
        return EnsureSize(slot, layer) ? slot : -1;
    }

    // tModLoader records a background's size while the mod loads, on a worker thread; a texture still loading then
    // is recorded as 0 × 0, and its layer draws nothing (middle) or throws (close: a DivideByZeroException in
    // DrawCloseBackground, which ends the whole surface background for that frame). Take the size from the texture
    // once it has loaded.
    private bool EnsureSize(int slot, string layer)
    {
        if (Main.backgroundWidth[slot] > 0 && Main.backgroundHeight[slot] > 0)
            return true;
        Asset<Texture2D> texture = TextureAssets.Background[slot];
        if (!texture.IsLoaded)
            return false;
        Main.backgroundWidth[slot] = texture.Width();
        Main.backgroundHeight[slot] = texture.Height();
        Mod.Logger.Info($"Background {Region}{layer} was registered as 0 x 0; size taken from the loaded texture.");
        return true;
    }

    private static int GroundRow(int slot)
    {
        if (!groundRows.TryGetValue(slot, out int row))
        {
            Texture2D texture = TextureAssets.Background[slot].Value;
            Color[] pixels = new Color[texture.Width * texture.Height];
            texture.GetData(pixels);
            byte[] alpha = System.Array.ConvertAll(pixels, pixel => pixel.A);
            groundRows[slot] = row = BackgroundLayoutRules.GroundRow(alpha, texture.Width, texture.Height);
        }
        return row;
    }

    // Fade this style in and every other one out, as vanilla does between biomes.
    public override void ModifyFarFades(float[] fades, float transitionSpeed)
    {
        for (int i = 0; i < fades.Length; i++)
            fades[i] = i == Slot ? System.Math.Min(1f, fades[i] + transitionSpeed) : System.Math.Max(0f, fades[i] - transitionSpeed);
    }

    public override int ChooseFarTexture() => Layer("Far");

    public override int ChooseMiddleTexture() => Layer("Mid");

    // Drawn where vanilla draws its own close layers, with the art's ground on the horizon (BackgroundLayoutRules).
    public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
    {
        int slot = Layer("Close");
        if (slot >= 0)
            b += BackgroundLayoutRules.CloseOffset(Main.screenHeight, Main.worldSurface, GroundRow(slot));
        return slot;
    }
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
