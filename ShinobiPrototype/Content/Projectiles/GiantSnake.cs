using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Content.Projectiles;

// Orochimaru's giant summoned snake (user, 2026-10-01): the ground shakes for a moment on one side of the screen, then
// a snake as tall as a house writhes across along the ground and is gone. Jump it or substitute. ai[0] is the way it
// goes (+1 east), ai[1] the ground row (world pixels) it crawls along.
public sealed class GiantSnake : ModProjectile
{
    private const int Segments = 16;
    private const float SegmentSize = 44f;
    private const float Speed = 15f;
    // The head starts this far behind where it was called (off screen) and crawls this far past.
    private const float StartBehind = 1100f;
    private const float Travel = 2600f;

    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    private int Dir => Projectile.ai[0] >= 0f ? 1 : -1;
    private float Ground => Projectile.ai[1];
    private int Tick => (int)Projectile.localAI[0];
    private bool Coming => Tick >= ExamBossRules.GiantSnakeWarnTicks;

    public override void SetDefaults()
    {
        Projectile.width = 60;
        Projectile.height = 60;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 600;
        Projectile.aiStyle = -1;
    }

    public override void AI()
    {
        Projectile.localAI[0]++;
        if (Tick == 1)
            Projectile.Center = new Vector2(Projectile.Center.X - Dir * StartBehind, Ground - SegmentSize / 2f);
        if (!Coming)
        {
            Projectile.velocity = Vector2.Zero;
            if (!Main.dedServ)
            {
                // The rumble: dust kicked up along the ground on the side it will come from, and the screen shakes.
                Vector2 edge = new(Projectile.Center.X + Dir * 300f, Ground);
                for (int i = 0; i < 3; i++)
                    Dust.NewDustPerfect(edge + new Vector2(Main.rand.NextFloat(-200f, 200f), -4f), DustID.Dirt,
                        new Vector2(0f, -Main.rand.NextFloat(1f, 3f)), 0, default, 1.3f);
                if (Tick % 20 == 1)
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Vector2.UnitY, 4f, 6f, 20, 2000f));
                if (Tick == 1)
                    SoundEngine.PlaySound(SoundID.Item69, Main.LocalPlayer.Center);
            }
            return;
        }
        if (Tick == ExamBossRules.GiantSnakeWarnTicks)
            SoundEngine.PlaySound(SoundID.Roar, Projectile.Center);
        Projectile.velocity = new Vector2(Dir * Speed, 0f);
        Projectile.Center = new Vector2(Projectile.Center.X, Ground - SegmentSize / 2f);
        if ((Tick - ExamBossRules.GiantSnakeWarnTicks) * Speed > Travel)
            Projectile.Kill();
    }

    private Vector2 HeadBottom => new(Projectile.Center.X, Ground);

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        if (!Coming)
            return false;
        float time = Tick;
        for (int i = 0; i < Segments; i++)
        {
            Vector2 at = SnakeBody.Segment(HeadBottom, Dir, i, Segments, SegmentSize, time);
            float s = SnakeBody.SizeAt(i, Segments, SegmentSize) * 0.85f;
            if (new Rectangle((int)(at.X - s / 2f), (int)(at.Y - s / 2f), (int)s, (int)s).Intersects(targetHitbox))
                return true;
        }
        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (!Coming)
            return false;
        Color light = Lighting.GetColor((int)(Projectile.Center.X / 16f), (int)(Ground / 16f) - 2);
        light = new Color(Math.Max(light.R, (byte)90), Math.Max(light.G, (byte)90), Math.Max(light.B, (byte)90));
        if (!DrawArt(light))
            SnakeBody.Draw(Main.spriteBatch, HeadBottom, Dir, Segments, SegmentSize, Tick, SnakeBody.Purple, light);
        return false;
    }

    // With the art (giant-snake-v1: head, one body segment, tail, all facing right): each part laid along the wave and
    // turned to follow it, tail first so the head is on top.
    private bool DrawArt(Color light)
    {
        const string root = "ShinobiPrototype/Content/Projectiles/GiantSnake_";
        if (!ModContent.HasAsset(root + "Head") || !ModContent.HasAsset(root + "Body") || !ModContent.HasAsset(root + "Tail"))
            return false;
        Texture2D head = ModContent.Request<Texture2D>(root + "Head").Value;
        Texture2D body = ModContent.Request<Texture2D>(root + "Body").Value;
        Texture2D tail = ModContent.Request<Texture2D>(root + "Tail").Value;
        SpriteEffects flip = Dir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        for (int i = Segments - 1; i >= 0; i--)
        {
            Vector2 at = SnakeBody.Segment(HeadBottom, Dir, i, Segments, SegmentSize, Tick);
            // The part points from the segment behind it towards the one ahead.
            Vector2 ahead = i == 0 ? at + new Vector2(Dir, 0f) : SnakeBody.Segment(HeadBottom, Dir, i - 1, Segments, SegmentSize, Tick);
            Vector2 along = ahead - at;
            float angle = (float)Math.Atan2(along.Y, along.X) - (Dir > 0 ? 0f : MathHelper.Pi);
            Texture2D part = i == 0 ? head : i == Segments - 1 ? tail : body;
            float scale = SnakeBody.SizeAt(i, Segments, SegmentSize) / SegmentSize * (i == 0 ? 1.1f : 1f);
            Main.EntitySpriteDraw(part, at - Main.screenPosition, null, light, angle, part.Size() / 2f, scale, flip);
        }
        return true;
    }
}
