using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;

namespace ShinobiPrototype.Content.Projectiles;

// The exam bosses' techniques (Dosu, Gaara, Neji, Orochimaru): one hostile hitbox whose kind (ai[0]) decides how it
// moves, what it looks like and what it does to the player it hits. ai[1] and ai[2] are its width and height in
// pixels. Drawn with dust only: placeholder visuals until the bosses have their art.
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
        Projectile.hide = true;
    }

    public static int Lifetime(JutsuKind kind) => kind switch
    {
        JutsuKind.Strike or JutsuKind.EchoDrill or JutsuKind.GentleFist => 12,
        JutsuKind.SoundWave or JutsuKind.WindBlast => 40,
        JutsuKind.SandShuriken => 120,
        JutsuKind.SandWave => 90,
        JutsuKind.SandBurial or JutsuKind.SixtyFour => 20,
        JutsuKind.AirBullet => 150,
        JutsuKind.SandArm => 22,
        JutsuKind.Rotation => ExamBossRules.RotationTicks,
        JutsuKind.SnakeHand => 44,
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
        }
        if (Main.netMode != NetmodeID.Server)
            Effects();
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
            case JutsuKind.SnakeHand:
                Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(box), DustID.Venom, Vector2.Zero, 0, default, 1.2f).noGravity = true;
                break;
            case JutsuKind.WindBlast:
                for (int i = 0; i < 4; i++)
                    Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(box), DustID.Cloud, Projectile.velocity * 0.4f, 100, default, 1.4f).noGravity = true;
                break;
        }
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
        }
    }
}
