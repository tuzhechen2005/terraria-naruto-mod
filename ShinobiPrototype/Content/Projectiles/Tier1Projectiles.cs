using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.Buffs;

namespace ShinobiPrototype.Content.Projectiles;

// The training ninjato's slash arc: it sweeps forward a short way and fades over three frames, hitting each enemy once.
// ai[0] = 1 when thrown from stealth (larger).
public sealed class NinjatoSlashArc : ModProjectile
{
    private const int Life = 18;

    public override string Texture => "ShinobiPrototype/Content/Projectiles/NinjatoSlashArc_0";

    public override void SetDefaults()
    {
        Projectile.width = 48;
        Projectile.height = 48;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = Life;
    }

    public override void AI()
    {
        if (Projectile.localAI[0] == 0f)
        {
            Projectile.localAI[0] = 1f;
            Projectile.scale = Projectile.ai[0] == 1f ? 1.5f : 1f;
            Projectile.Resize((int)(48 * Projectile.scale), (int)(48 * Projectile.scale));
        }
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.velocity *= 0.92f;
        Lighting.AddLight(Projectile.Center, 0.25f, 0.35f, 0.5f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        int frame = System.Math.Min(2, (Life - Projectile.timeLeft) * 3 / Life);
        Texture2D art = ModContent.Request<Texture2D>($"ShinobiPrototype/Content/Projectiles/NinjatoSlashArc_{frame}").Value;
        SpriteEffects flip = Projectile.velocity.X < 0f ? SpriteEffects.FlipVertically : SpriteEffects.None;
        Main.EntitySpriteDraw(art, Projectile.Center - Main.screenPosition, null, Color.White * 0.9f, Projectile.rotation,
            art.Size() / 2f, Projectile.scale, flip);
        return false;
    }
}

// The chain gauntlet's claw. Out to its reach, then back; it hooks the first enemy (dragging it toward the player
// with a strong knockback and poisoning it) or a wall (pulling the player there).
public sealed class GauntletClaw : ModProjectile
{
    public const float ReachPx = 18 * 16f;
    private const float ReturnSpeed = 18f;
    private const float PullSpeed = 12f;
    private const int PullTicks = 30;

    private ref float State => ref Projectile.ai[0];   // 0 out, 1 back, 2 pulling the player to a wall
    private ref float Timer => ref Projectile.ai[1];

    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.timeLeft = 600;
    }

    public override bool? CanDamage() => State == 0f ? null : false;

    public override void AI()
    {
        Player owner = Main.player[Projectile.owner];
        if (!owner.active || owner.dead)
        {
            Projectile.Kill();
            return;
        }
        Vector2 toOwner = owner.Center - Projectile.Center;
        Projectile.rotation = (-toOwner).ToRotation();
        switch (State)
        {
            case 0f:
                if (toOwner.Length() > ReachPx)
                    Back();
                break;
            case 2f:
                Projectile.velocity = Vector2.Zero;
                if (Projectile.owner == Main.myPlayer)
                {
                    owner.velocity = Vector2.Normalize(-toOwner) * PullSpeed;
                    owner.fallStart = (int)(owner.position.Y / 16f);
                }
                if (++Timer > PullTicks || toOwner.Length() < 40f)
                    Back();
                break;
            default:
                Projectile.tileCollide = false;
                Projectile.velocity = Vector2.Normalize(toOwner) * ReturnSpeed;
                if (toOwner.Length() < 24f)
                    Projectile.Kill();
                break;
        }
        owner.itemTime = owner.itemAnimation = 2;
        owner.ChangeDir(Projectile.Center.X > owner.Center.X ? 1 : -1);
    }

    private void Back()
    {
        State = 1f;
        Timer = 0f;
        Projectile.netUpdate = true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (State == 0f)
        {
            State = 2f;
            Timer = 0f;
            Projectile.tileCollide = false;
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            Projectile.netUpdate = true;
        }
        return false;
    }

    // Dragged toward the player: knockback pointing back at them (bosses and the unmovable shrug it off).
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.HitDirectionOverride = Main.player[Projectile.owner].Center.X < target.Center.X ? -1 : 1;
        modifiers.Knockback *= 2f;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Poisoned, 4 * 60);
        Back();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Player owner = Main.player[Projectile.owner];
        Texture2D link = ModContent.Request<Texture2D>("ShinobiPrototype/Content/Projectiles/GauntletChainLink").Value;
        Vector2 from = owner.MountedCenter;
        Vector2 along = Projectile.Center - from;
        float length = along.Length();
        float rotation = along.ToRotation() + MathHelper.PiOver2;
        for (float d = 0f; d < length; d += link.Height)
        {
            Vector2 at = from + along * (d / length);
            Main.EntitySpriteDraw(link, at - Main.screenPosition, null, Lighting.GetColor(at.ToTileCoordinates()), rotation,
                link.Size() / 2f, 1f, SpriteEffects.None);
        }
        Texture2D claw = ModContent.Request<Texture2D>(Texture).Value;
        Main.EntitySpriteDraw(claw, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation,
            claw.Size() / 2f, 1f, SpriteEffects.None);
        return false;
    }
}

// 凤仙火: a small fireball; half the time it sets the enemy alight.
public sealed class PhoenixFlowerFire : ModProjectile
{
    public override string Texture => "ShinobiPrototype/Content/Projectiles/PhoenixFlowerFire_0";

    public override void SetDefaults()
    {
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 50;
    }

    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation();
        Lighting.AddLight(Projectile.Center, 0.8f, 0.45f, 0.1f);
        if (Main.rand.NextBool(3))
            Dust.NewDustPerfect(Projectile.Center, DustID.Torch, -Projectile.velocity * 0.2f, 0, default, 1.1f).noGravity = true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Main.rand.NextBool(2))
            target.AddBuff(BuffID.OnFire, 3 * 60);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D art = ModContent.Request<Texture2D>(
            $"ShinobiPrototype/Content/Projectiles/PhoenixFlowerFire_{Main.GameUpdateCount / 4 % 4}").Value;
        Main.EntitySpriteDraw(art, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, art.Size() / 2f,
            1f, SpriteEffects.None);
        return false;
    }
}

// Pakkun, Kakashi's ninken: vanilla's flinx minion AI (runs on the ground, jumps, chases and bites), drawn from his own
// frames. A bite leaves his scent on the enemy (PakkunScent).
public sealed class PakkunMinion : ModProjectile
{
    private const int BiteShowTicks = 14;

    public override string Texture => "ShinobiPrototype/Content/Projectiles/Pakkun_Idle";

    public override void SetStaticDefaults()
    {
        Main.projPet[Type] = true;
        ProjectileID.Sets.MinionSacrificable[Type] = true;
        ProjectileID.Sets.MinionTargettingFeature[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.FlinxMinion);
        AIType = ProjectileID.FlinxMinion;
        Projectile.width = 28;
        Projectile.height = 22;
    }

    public override bool? CanCutTiles() => false;

    public override bool MinionContactDamage() => true;

    public override void PostAI()
    {
        Player owner = Main.player[Projectile.owner];
        if (owner.dead || !owner.active)
            owner.ClearBuff(ModContent.BuffType<PakkunBuff>());
        if (owner.HasBuff(ModContent.BuffType<PakkunBuff>()))
            Projectile.timeLeft = 2;
        if (Projectile.localAI[1] > 0f)
            Projectile.localAI[1]--;
        if (System.Math.Abs(Projectile.velocity.X) > 0.2f)
            Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(ModContent.BuffType<PakkunScent>(), PakkunScent.Ticks);
        Projectile.localAI[1] = BiteShowTicks;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        string frame = Projectile.localAI[1] > 0f ? $"Pakkun_Bite_{(Projectile.localAI[1] > BiteShowTicks / 2 ? 0 : 1)}"
            : System.Math.Abs(Projectile.velocity.X) > 0.4f ? $"Pakkun_Run_{Main.GameUpdateCount / 6 % 4}"
            : "Pakkun_Idle";
        Texture2D art = ModContent.Request<Texture2D>($"ShinobiPrototype/Content/Projectiles/{frame}").Value;
        Vector2 feet = Projectile.Bottom - Main.screenPosition + new Vector2(0f, 2f);
        Main.EntitySpriteDraw(art, feet, null, lightColor, 0f, new Vector2(art.Width / 2f, art.Height), 1f,
            Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
        return false;
    }
}

// A kunai thrown by a rogue genin: flies straight, a little gravity late in its flight.
public sealed class HostileKunai : ModProjectile
{
    public override string Texture => "ShinobiPrototype/Content/Items/TrainingKunai";

    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.hostile = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 120;
        Projectile.aiStyle = -1;
    }

    public override void AI()
    {
        if (++Projectile.ai[0] > 30f)
            Projectile.velocity.Y += 0.15f;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;   // the kunai art lies diagonally, like KunaiThrown
    }
}
