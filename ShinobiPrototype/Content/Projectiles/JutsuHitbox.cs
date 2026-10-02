using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;

namespace ShinobiPrototype.Content.Projectiles;

// The exam bosses' techniques (Dosu, Gaara, Neji, Orochimaru): one hostile hitbox whose kind (ai[0]) decides how it
// moves, what it looks like and what it does to the player it hits. ai[1] and ai[2] are its width and height in
// pixels. Some techniques are drawn from their own art when it is there (FxArt: Orochimaru's snake hands, gust and
// bite; Gaara's sand shuriken and sand wave); everything else is still dust.
public enum JutsuKind : byte
{
    Strike,        // a plain melee hit
    EchoDrill,     // Dosu: the ears ring (left and right swap)
    SoundWave,     // Dosu: a cone that throws the player back
    SandShuriken,  // Gaara
    SandWave,      // Gaara: rolls along the ground
    SandBurial,    // Gaara: the coffin closes
    AirBullet,     // Gaara transformed: big and slow
    SandArm,       // Gaara transformed: a long sweep
    GentleFist,    // Neji: seals a chakra point
    Rotation,      // Neji: the spinning dome
    SixtyFour,     // Neji: every point sealed
    SnakeHand,     // Orochimaru: shoots out and comes back
    WindBlast,     // Orochimaru: throws the player back
    FiveSeal,      // Orochimaru: chakra stops recovering
    GroundQuake,   // Dosu: a sound wave crawling along the floor (jump it)
    ResonanceRing, // Dosu: a ring of sound spreading out from him (only the ring itself hurts)
    ImpactSlam,    // Dosu: the shock of landing from his leap
    Bite,          // Orochimaru: the snake dash and the long neck close their fangs
    NeckHead,      // Orochimaru: rides on the head of his long neck while it lunges
}

public sealed class JutsuHitbox : ModProjectile
{
    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    private JutsuKind Kind => (JutsuKind)(int)Projectile.ai[0];

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 30;
        Projectile.aiStyle = -1;
    }

    public static int Lifetime(JutsuKind kind) => kind switch
    {
        JutsuKind.Strike or JutsuKind.EchoDrill or JutsuKind.GentleFist or JutsuKind.FiveSeal or JutsuKind.Bite => 12,
        JutsuKind.SoundWave => 40,
        // The gust rolls on off the screen (user, 2026-10-02: it vanished after a short way).
        JutsuKind.WindBlast => 110,
        JutsuKind.NeckHead => 26,
        JutsuKind.SandShuriken => 120,
        JutsuKind.SandWave => 90,
        JutsuKind.SandBurial or JutsuKind.SixtyFour => 20,
        JutsuKind.AirBullet => 150,
        JutsuKind.SandArm => 22,
        JutsuKind.Rotation => ExamBossRules.RotationTicks,
        JutsuKind.SnakeHand => 40,
        JutsuKind.GroundQuake => 75,
        JutsuKind.ResonanceRing => 50,
        JutsuKind.ImpactSlam => 14,
        _ => 30,
    };

    // Spawns one; only the server (or single player) should call this.
    public static void Spawn(NPC npc, JutsuKind kind, Vector2 center, Vector2 velocity, int width, int height, int damage,
        float knockback = 3f)
    {
        Projectile.NewProjectile(npc.GetSource_FromAI(), center, velocity, ModContent.ProjectileType<JutsuHitbox>(),
            damage, knockback, Main.myPlayer, (float)kind, width, height);
    }

    public override void AI()
    {
        if (Projectile.localAI[0] == 0f)
        {
            Projectile.localAI[0] = 1f;
            // Every machine sets these itself on the first tick: players are hurt on their own clients.
            Projectile.Resize((int)Projectile.ai[1], (int)Projectile.ai[2]);
            Projectile.timeLeft = Lifetime(Kind);
            Projectile.localAI[1] = Projectile.timeLeft;
            // The snake hands and the neck remember whose they are (each machine for itself).
            if (Kind is JutsuKind.SnakeHand or JutsuKind.NeckHead)
            {
                NPC owner = null;
                foreach (NPC npc in Main.ActiveNPCs)
                    if (npc.type == ModContent.NPCType<NPCs.Orochimaru>() &&
                        (owner == null || npc.Distance(Projectile.Center) < owner.Distance(Projectile.Center)))
                        owner = npc;
                Projectile.localAI[2] = owner?.whoAmI + 1 ?? 0;
            }
        }
        switch (Kind)
        {
            case JutsuKind.SnakeHand:
                // Out for the first half, back for the second.
                if (Projectile.timeLeft == (int)(Projectile.localAI[1] / 2f))
                    Projectile.velocity = -Projectile.velocity;
                break;
            case JutsuKind.SandWave:
                Projectile.velocity.Y = 0f;
                break;
            case JutsuKind.SandShuriken:
                Projectile.rotation += 0.4f;
                break;
            case JutsuKind.GroundQuake:
                Projectile.velocity.Y = 0f;
                break;
            case JutsuKind.WindBlast:
            {
                // It grows as it goes.
                float t = 1f - Projectile.timeLeft / Projectile.localAI[1];
                Vector2 center = Projectile.Center;
                Projectile.Resize((int)(Projectile.ai[1] * (1f + t)), (int)(Projectile.ai[2] * (1f + 0.6f * t)));
                Projectile.Center = center;
                break;
            }
            case JutsuKind.NeckHead:
                if (Projectile.localAI[2] > 0f && Main.npc[(int)Projectile.localAI[2] - 1] is { active: true, ModNPC: NPCs.Orochimaru o })
                    Projectile.Center = o.NeckHead;
                else
                    Projectile.Kill();
                Projectile.velocity = Vector2.Zero;
                break;
            case JutsuKind.ResonanceRing:
            {
                // Grows from the caster out to its full size (ai[1]) over its life, keeping its centre.
                float t = 1f - Projectile.timeLeft / Projectile.localAI[1];
                int size = (int)MathHelper.Lerp(40f, Projectile.ai[1], t);
                Vector2 center = Projectile.Center;
                Projectile.Resize(size, size);
                Projectile.Center = center;
                break;
            }
        }
        if (Main.netMode != NetmodeID.Server && !DrawsArt)
            Effects();
    }

    // Kinds with their own art (and the art present) skip the dust.
    private bool DrawsArt => Kind switch
    {
        JutsuKind.SnakeHand => FxArt.Has("FxSnakeHand_Head") && FxArt.Has("FxSnakeHand_Segment"),
        JutsuKind.WindBlast => FxArt.Has("FxWind_0"),
        JutsuKind.Bite => FxArt.Has("FxBite_0"),
        JutsuKind.SandShuriken => FxArt.Has("FxSandShuriken_0"),
        JutsuKind.SandWave => FxArt.Has("FxSandWave_0"),
        _ => false,
    };

    public override bool PreDraw(ref Color lightColor)
    {
        if (!DrawsArt)
            return false;
        Color light = NPCs.BossSprites.Lit(lightColor, 0.55f);
        int age = (int)(Projectile.localAI[1] - Projectile.timeLeft);
        switch (Kind)
        {
            case JutsuKind.SnakeHand:
                DrawSnakeHand(light);
                break;
            case JutsuKind.WindBlast:
                // The gust, stretched over the hitbox, rolling forward.
                Microsoft.Xna.Framework.Graphics.Texture2D gust = FxArt.Frame("FxWind", age / 6, 4);
                float scale = Projectile.width / (float)gust.Width * 1.25f;
                FxArt.Draw(gust, Projectile.Center, light, 0f, scale, Projectile.velocity.X >= 0f ? 1 : -1);
                break;
            case JutsuKind.Bite:
                Microsoft.Xna.Framework.Graphics.Texture2D bite = FxArt.Frame("FxBite", age / 4, 3) ?? FxArt.Get("FxBite_0");
                FxArt.Draw(bite, Projectile.Center, light, 0f, 1.5f);
                break;
            case JutsuKind.SandShuriken:
                FxArt.Draw(FxArt.Frame("FxSandShuriken", age / 4, 2), Projectile.Center, light, Projectile.rotation, 1.5f);
                break;
            case JutsuKind.SandWave:
                // Rolling along the ground, its foot on the bottom of the hitbox: it swells (frames 0, 1), then curls
                // over and over (1, 2).
                Microsoft.Xna.Framework.Graphics.Texture2D wave = FxArt.Frame("FxSandWave", age < 6 ? 0 : 1 + age / 7 % 2, 3);
                FxArt.Draw(wave, Projectile.Bottom - new Vector2(0f, wave.Height * 0.75f), light, 0f, 1.5f,
                    Projectile.velocity.X >= 0f ? 1 : -1);
                break;
        }
        return false;
    }

    // A snake from Orochimaru's sleeve out to the hitbox: body segments along the line, writhing, the head at the end.
    private void DrawSnakeHand(Color light)
    {
        NPC owner = Projectile.localAI[2] > 0f ? Main.npc[(int)Projectile.localAI[2] - 1] : null;
        if (owner is not { active: true })
            return;
        // From his raised hand (orochimaru-set-v11a: he stands with it up while the snakes are out).
        Vector2 from = owner.Center + new Vector2(owner.direction * 20f, -36f);
        Vector2 to = Projectile.Center;
        Vector2 line = to - from;
        float length = line.Length();
        if (length < 4f)
            return;
        Vector2 dir = line / length, side = new(-dir.Y, dir.X);
        Microsoft.Xna.Framework.Graphics.Texture2D segment = FxArt.Get("FxSnakeHand_Segment");
        Microsoft.Xna.Framework.Graphics.Texture2D head = FxArt.Get("FxSnakeHand_Head");
        float step = segment.Width * 1.5f * 0.7f;
        float time = Main.GameUpdateCount + Projectile.whoAmI * 7;
        for (float d = 0f; d < length - step; d += step)
        {
            float wave = (float)System.Math.Sin(d * 0.06f - time * 0.4f) * 6f * System.Math.Min(1f, d / 40f);
            FxArt.Draw(segment, from + dir * d + side * wave, light, (float)System.Math.Atan2(dir.Y, dir.X), 1.5f);
        }
        bool left = dir.X < 0f;
        float angle = (float)System.Math.Atan2(dir.Y, dir.X);
        FxArt.Draw(head, to, light, left ? angle - MathHelper.Pi : angle, 1.5f, left ? -1 : 1);
    }

    private void Effects()
    {
        Rectangle box = Projectile.Hitbox;
        switch (Kind)
        {
            case JutsuKind.EchoDrill:
            case JutsuKind.SoundWave:
                for (int i = 0; i < 3; i++)
                    Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(box), DustID.Smoke, Projectile.velocity * 0.2f, 120,
                        new Color(220, 220, 255), 1.3f).noGravity = true;
                break;
            case JutsuKind.SandShuriken:
            case JutsuKind.SandWave:
            case JutsuKind.SandBurial:
            case JutsuKind.SandArm:
            case JutsuKind.AirBullet:
                int count = Kind is JutsuKind.SandShuriken ? 1 : 5;
                for (int i = 0; i < count; i++)
                    Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(box), Kind == JutsuKind.AirBullet ? DustID.Cloud : DustID.Sand,
                        Projectile.velocity * 0.1f + Main.rand.NextVector2Circular(1f, 1f), 60, default, 1.3f).noGravity = true;
                break;
            case JutsuKind.GentleFist:
            case JutsuKind.Rotation:
            case JutsuKind.SixtyFour:
                int chakra = Kind == JutsuKind.Rotation ? 6 : 3;
                for (int i = 0; i < chakra; i++)
                {
                    Vector2 at = Kind == JutsuKind.Rotation
                        ? Projectile.Center + Main.rand.NextVector2CircularEdge(Projectile.width / 2f, Projectile.height / 2f)
                        : Main.rand.NextVector2FromRectangle(box);
                    Dust.NewDustPerfect(at, DustID.IceTorch, Vector2.Zero, 0, default, 1.4f).noGravity = true;
                }
                break;
            case JutsuKind.FiveSeal:
                for (int i = 0; i < 4; i++)
                    Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(box), DustID.PurpleTorch, Vector2.Zero, 0, default, 1.5f)
                        .noGravity = true;
                break;
            case JutsuKind.SnakeHand:
                Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(box), DustID.Venom, Vector2.Zero, 0, default, 1.2f).noGravity = true;
                break;
            case JutsuKind.WindBlast:
                for (int i = 0; i < 4; i++)
                    Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(box), DustID.Cloud, Projectile.velocity * 0.4f, 100, default, 1.4f).noGravity = true;
                break;
            case JutsuKind.GroundQuake:
                for (int i = 0; i < 3; i++)
                    Dust.NewDustPerfect(new Vector2(Main.rand.NextFloat(box.Left, box.Right), box.Bottom - Main.rand.NextFloat(0f, box.Height)),
                        DustID.Smoke, new Vector2(0f, -1.5f), 100, new Color(220, 220, 255), 1.4f).noGravity = true;
                break;
            case JutsuKind.ResonanceRing:
                for (int i = 0; i < 10; i++)
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(Projectile.width / 2f, Projectile.height / 2f),
                        DustID.Smoke, Vector2.Zero, 100, new Color(220, 220, 255), 1.3f).noGravity = true;
                break;
            case JutsuKind.ImpactSlam:
                for (int i = 0; i < 6; i++)
                    Dust.NewDustPerfect(new Vector2(Main.rand.NextFloat(box.Left, box.Right), box.Bottom), DustID.Smoke,
                        new Vector2(Main.rand.NextFloat(-3f, 3f), -2f), 80, default, 1.6f);
                break;
        }
    }

    // The ring only hurts at its edge: inside it, the sound has already passed.
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        if (Kind != JutsuKind.ResonanceRing)
            return null;
        float radius = Projectile.width / 2f;
        Vector2 nearest = Vector2.Clamp(Projectile.Center, targetHitbox.TopLeft(), targetHitbox.BottomRight());
        float distance = Vector2.Distance(nearest, Projectile.Center);
        float farthest = 0f;
        foreach (Vector2 corner in new[] { targetHitbox.TopLeft(), targetHitbox.TopRight(), targetHitbox.BottomLeft(), targetHitbox.BottomRight() })
            farthest = System.Math.Max(farthest, Vector2.Distance(corner, Projectile.Center));
        return distance <= radius && farthest >= radius - 28f;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        JutsuStatusPlayer status = target.GetModPlayer<JutsuStatusPlayer>();
        switch (Kind)
        {
            case JutsuKind.EchoDrill:
                status.Ring(ExamBossRules.TinnitusTicks);
                break;
            case JutsuKind.GentleFist:
                status.Seal();
                break;
            case JutsuKind.SixtyFour:
                for (int i = 0; i < ExamBossRules.SealMaxStacks; i++)
                    status.Seal();
                break;
            case JutsuKind.FiveSeal:
                status.SealRegen(ExamBossRules.FiveSealTicks);
                status.ShowSealGlyph();
                break;
        }
    }
}
