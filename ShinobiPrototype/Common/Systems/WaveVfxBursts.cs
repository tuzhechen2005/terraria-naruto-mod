using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Systems;

// Client-only, bounded decorative aftermath. No Projectile/NPC slots, collision
// or network messages. Attack logic explicitly requests the four M11 impacts.
public sealed class WaveVfxBursts : ModSystem
{
    public enum Kind { Splash, MirrorShatter, ChakraCatch, Frost }
    public enum Impact { HeavySlash, DragonRelease, CageBroken, FrenzyStart }
    public const int Capacity = 64;
    private readonly List<Burst> bursts = new(Capacity);
    private readonly record struct Burst(Kind Kind, Vector2 Position, float Size, int Age);
    private int shakeTicks;
    private int darkTicks;
    private Vector2 impactPosition;

    public static void Spawn(Kind kind, Vector2 center, float size = 100f)
    {
        if (Main.dedServ || Main.gameMenu || ShinobiClientConfig.Instance.WaveVfxStrength <= 0 ||
            !float.IsFinite(center.X) || !float.IsFinite(center.Y) || !float.IsFinite(size))
            return;
        var system = ModContent.GetInstance<WaveVfxBursts>();
        if (system.bursts.Count == Capacity)
            system.bursts.RemoveAt(0);
        system.bursts.Add(new Burst(kind, center, Math.Clamp(size, 8f, 512f), 0));
    }

    public static void Pulse(Impact kind, Vector2 center)
    {
        if (Main.dedServ || Main.gameMenu || ShinobiClientConfig.Instance.WaveVfxStrength <= 0)
            return;
        // Remote/offscreen fights must not shake another player's view.
        if (Vector2.DistanceSquared(Main.LocalPlayer.Center, center) > 1200f * 1200f)
            return;
        var system = ModContent.GetInstance<WaveVfxBursts>();
        // Do not stack amplitude when multiple clients' actors hit together.
        system.shakeTicks = Math.Max(system.shakeTicks, 8); // 0.133 seconds
        if (kind != Impact.HeavySlash)
            system.darkTicks = Math.Max(system.darkTicks, 18); // 0.3 seconds
        system.impactPosition = center;
    }

    public override void PostUpdateEverything()
    {
        if (Main.dedServ)
            return;
        for (int i = bursts.Count - 1; i >= 0; i--)
        {
            var burst = bursts[i];
            if (burst.Age >= 23)
                bursts.RemoveAt(i);
            else
                bursts[i] = burst with { Age = burst.Age + 1 };
        }
        shakeTicks = Math.Max(0, shakeTicks - 1);
        darkTicks = Math.Max(0, darkTicks - 1);
    }

    public override void ModifyScreenPosition()
    {
        if (Main.dedServ || Main.gameMenu || shakeTicks <= 0 ||
            !ShinobiClientConfig.Instance.WaveScreenShake)
            return;
        float amount = 3f * shakeTicks / 8f * ShinobiClientConfig.Instance.WaveVfxStrength / 100f;
        float phase = shakeTicks * 2.4f;
        Main.screenPosition += new Vector2((float)Math.Sin(phase), (float)Math.Cos(phase * 1.3f)) * amount;
    }

    public override void PostDrawTiles()
    {
        if (Main.dedServ || Main.gameMenu || (bursts.Count == 0 && darkTicks == 0))
            return;
        var batch = Main.spriteBatch;
        batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
            DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        // Background/tiles pass; ordinary foreground actors and UI are drawn
        // later. No full-screen white flash or input/camera lock.
        if (darkTicks > 0 && ShinobiClientConfig.Instance.WaveBackgroundDim &&
            Vector2.DistanceSquared(Main.LocalPlayer.Center, impactPosition) <= 1200f * 1200f)
        {
            Vector2 zoom = Main.GameViewMatrix.Zoom;
            Vector2 view = new Vector2(Main.screenWidth, Main.screenHeight) / zoom;
            Vector2 topLeft = (new Vector2(Main.screenWidth, Main.screenHeight) - view) * 0.5f;
            float fade = Math.Min(darkTicks / 6f, (18 - darkTicks) / 3f);
            batch.Draw(TextureAssets.MagicPixel.Value, topLeft - Vector2.One * 8f,
                new Rectangle(0, 0, 1, 1),
                Color.Black * (0.2f * Math.Clamp(fade, 0f, 1f) *
                    ShinobiClientConfig.Instance.WaveVfxStrength / 100f), 0f, Vector2.Zero,
                view + Vector2.One * 16f, SpriteEffects.None, 0f);
        }
        foreach (var burst in bursts)
        {
            float phase = burst.Age / 24f;
            switch (burst.Kind)
            {
                case Kind.Splash: WaveVfx.Splash(batch, burst.Position, phase, burst.Size); break;
                case Kind.MirrorShatter: WaveVfx.Shatter(batch, burst.Position, phase, burst.Size); break;
                case Kind.ChakraCatch: WaveVfx.ChakraImpact(batch, burst.Position, phase, burst.Size); break;
                case Kind.Frost: WaveVfx.Shatter(batch, burst.Position, phase, burst.Size * 0.5f); break;
            }
        }
        batch.End();
    }

    public override void OnWorldUnload()
    {
        bursts.Clear();
        shakeTicks = darkTicks = 0;
        impactPosition = Vector2.Zero;
    }

    public override void Unload() => OnWorldUnload();
}
