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

// Orochimaru's giant summoned snake, Manda (user, 2026-10-01): the ground shakes for a moment on one side of the
// screen, then a snake as long as the screen is wide slides across along the ground and is gone. Too thick to jump
// without help: substitute, or get up high. ai[0] is the way it goes (+1 east), ai[1] the ground row (world pixels).
public sealed class GiantSnake : ModProjectile
{
    // Long and thick, sliding along the ground (user, 2026-10-01: about a quarter of the view above the ground, and
    // as long as the screen is wide). The parts (giant-snake-v3) are drawn at whole multiples of their 2x2 art pixels.
    private const int Segments = 24;
    private const float PartScale = 3.5f;   // 2-pixel art blocks become 7 screen pixels
    private const float HeadScale = 4f;
    private const float BodyWidth = 56f, BodyHeight = 42f, HeadHeight = 62f;
    private const float Spacing = BodyWidth * PartScale * 0.62f;
    private const float Speed = 15f;
    // The head starts this far behind where it was called (off screen) and crawls on until the tail is well past.
    private const float StartBehind = 1100f;
    private const float Travel = StartBehind * 2f + Segments * Spacing;

    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    private int Dir => Projectile.ai[0] >= 0f ? 1 : -1;
    private float Ground => Projectile.ai[1];
    private int Tick => (int)Projectile.localAI[0];
    private bool Coming => Tick >= ExamBossRules.GiantSnakeWarnTicks;

    // Vanilla stops drawing a projectile once its hitbox (here the head) is this far off screen; the body trails a
    // screen's width behind the head, so keep drawing until the tail is gone too (user, 2026-10-01: it vanished
    // halfway across).
    public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Type] = (int)(Segments * Spacing) + 800;

    public override void SetDefaults()
    {
        Projectile.width = 60;
        Projectile.height = 60;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 900;
        Projectile.aiStyle = -1;
    }

    public override void AI()
    {
        Projectile.localAI[0]++;
        if (Tick == 1)
            Projectile.Center = new Vector2(Projectile.Center.X - Dir * StartBehind, Ground - 40f);
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
        Projectile.Center = new Vector2(Projectile.Center.X, Ground - 40f);
        if ((Tick - ExamBossRules.GiantSnakeWarnTicks) * Speed > Travel)
            Projectile.Kill();
    }

    private float Scale(int i) => i == 0 ? HeadScale : PartScale * (1f - 0.35f * (float)Math.Pow(i / (float)Segments, 1.6));

    // Each part's centre: behind the head along the ground, resting on it, with a low ripple running back the body.
    private Vector2 Part(int i)
    {
        float height = (i == 0 ? HeadHeight : BodyHeight) * Scale(i);
        float ripple = Math.Max(0f, (float)Math.Sin(Tick * 0.2f - i * 0.6f)) * 0.12f * BodyHeight * PartScale * Math.Min(1f, i / 3f);
        return new Vector2(Projectile.Center.X - Dir * i * Spacing, Ground - height / 2f - ripple);
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        if (!Coming)
            return false;
        for (int i = 0; i < Segments; i++)
        {
            Vector2 at = Part(i);
            float w = BodyWidth * Scale(i) * 0.9f, h = (i == 0 ? HeadHeight : BodyHeight) * Scale(i) * 0.85f;
            if (new Rectangle((int)(at.X - w / 2f), (int)(at.Y - h / 2f), (int)w, (int)h).Intersects(targetHitbox))
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
            SnakeBody.Draw(Main.spriteBatch, new Vector2(Projectile.Center.X, Ground), Dir, Segments, BodyHeight * PartScale, Tick,
                SnakeBody.Purple, light);
        return false;
    }

    // Each part (head, body segments, tail, all facing right) laid along the ground and turned to follow the ripple,
    // tail first so the head is on top.
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
            Vector2 at = Part(i);
            Vector2 along = (i == 0 ? at + new Vector2(Dir, 0f) : Part(i - 1)) - at;
            float angle = (float)Math.Atan2(along.Y, along.X) - (Dir > 0 ? 0f : MathHelper.Pi);
            Texture2D part = i == 0 ? head : i == Segments - 1 ? tail : body;
            Vector2 position = new((float)Math.Round(at.X - Main.screenPosition.X), (float)Math.Round(at.Y - Main.screenPosition.Y));
            // Each part lit where it is, never darker than a dim daylight (user, 2026-10-02: it turned black as a whole
            // when the head passed through the dark).
            Color partLight = BossSprites.Lit(Lighting.GetColor((int)(at.X / 16f), (int)(at.Y / 16f)), 0.6f);
            Main.EntitySpriteDraw(part, position, null, partLight, angle, part.Size() / 2f, Scale(i), flip);
        }
        return true;
    }
}
