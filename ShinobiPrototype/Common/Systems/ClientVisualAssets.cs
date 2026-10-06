using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Systems;

// Extra animation frames and VFX are not the content type's default texture, so tML does not preload them for us.
// Request them asynchronously during mod loading, where tML waits for completion before entering gameplay.
// Retain shared Asset handles; soft Wave art has private premultiplied copies released on unload.
public sealed class ClientVisualAssets : ModSystem
{
    public const string Projectiles = "Content/Projectiles";
    public const string Npcs = "Content/NPCs";
    private readonly Dictionary<(string Folder, string Name), Asset<Texture2D>> textures = new();
    private readonly Dictionary<(string Folder, string Name, int Count), Asset<Texture2D>[]> animations = new();
    private readonly Dictionary<string, Texture2D> waveTextures = new();

    public override void Load()
    {
        if (Main.dedServ)
            return;
        foreach (string file in Mod.GetFileNames())
        {
            // tML packs PNGs as rawimg; PNG is accepted too for development/content-source compatibility.
            int slash = file.LastIndexOf('/');
            int dot = file.LastIndexOf('.');
            if (slash < 0 || dot <= slash || !(file.EndsWith(".rawimg", StringComparison.OrdinalIgnoreCase) ||
                                             file.EndsWith(".png", StringComparison.OrdinalIgnoreCase)))
                continue;
            string folder = file[..slash];
            if (folder != Projectiles && folder != Npcs && folder != "Assets/UI" && folder != WaveVfx.Folder)
                continue;
            string name = file[(slash + 1)..dot];
            textures[(folder, name)] = Mod.Assets.Request<Texture2D>(file[..dot], AssetRequestMode.AsyncLoad);
        }
    }

    public override void PostSetupContent()
    {
        if (Main.dedServ)
            return;
        var sources = new List<(string Name, Texture2D Texture)>();
        foreach (var entry in textures)
            if (entry.Key.Folder == WaveVfx.Folder && !waveTextures.ContainsKey(entry.Key.Name))
                sources.Add((entry.Key.Name, Mod.Assets.Request<Texture2D>(
                    $"{entry.Key.Folder}/{entry.Key.Name}", AssetRequestMode.ImmediateLoad).Value));
        // rawimg loading preserves straight RGBA. Prepare dedicated premultiplied
        // copies once on the graphics thread; never modify shared Asset textures.
        Main.RunOnMainThread(() =>
        {
            foreach (var entry in sources)
            {
                Texture2D source = entry.Texture;
                var pixels = new Color[source.Width * source.Height];
                source.GetData(pixels);
                for (int i = 0; i < pixels.Length; i++)
                {
                    Color c = pixels[i];
                    pixels[i] = new Color((c.R * c.A + 127) / 255,
                        (c.G * c.A + 127) / 255,
                        (c.B * c.A + 127) / 255, c.A);
                }
                var converted = new Texture2D(Main.instance.GraphicsDevice, source.Width, source.Height);
                converted.SetData(pixels);
                waveTextures.Add(entry.Name, converted);
            }
        }).GetAwaiter().GetResult();
    }

    public override void Unload()
    {
        if (waveTextures.Count > 0)
            Main.RunOnMainThread(() =>
            {
                foreach (Texture2D texture in waveTextures.Values)
                    texture.Dispose();
                waveTextures.Clear();
            }).GetAwaiter().GetResult();
        animations.Clear();
        textures.Clear();
    }

    public static bool Has(string folder, string name) =>
        !Main.dedServ && ModContent.GetInstance<ClientVisualAssets>().textures.ContainsKey((folder, name));

    public static Texture2D Get(string folder, string name)
    {
        if (!Main.dedServ && folder == WaveVfx.Folder)
            return ModContent.GetInstance<ClientVisualAssets>().waveTextures.GetValueOrDefault(name);
        if (!Main.dedServ && ModContent.GetInstance<ClientVisualAssets>().textures.TryGetValue((folder, name), out var asset)
            && asset.IsLoaded)
            return asset.Value;
        return null;
    }

    public static Texture2D Frame(string folder, string name, int frame, int count)
    {
        if (Main.dedServ || count <= 0)
            return null;
        var cache = ModContent.GetInstance<ClientVisualAssets>();
        var key = (folder, name, count);
        if (!cache.animations.TryGetValue(key, out var strip))
        {
            strip = new Asset<Texture2D>[count];
            for (int i = 0; i < count; i++)
                cache.textures.TryGetValue((folder, $"{name}_{i}"), out strip[i]);
            cache.animations[key] = strip;
        }
        var asset = strip[((frame % count) + count) % count];
        return asset?.IsLoaded == true ? asset.Value : null;
    }
}
