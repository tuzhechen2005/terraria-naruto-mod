using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;

namespace ShinobiPrototype.Content.Projectiles;

// Kakashi's drill kunai: held up for a moment as a warning, then thrown at the player who asked for practice.
// It never deals damage; touching the player either triggers a Substitution (if one is on standby) or just ends.
public sealed class KakashiPracticeKunai : ModProjectile
{
    private const int WindupTicks = 50;
    private const float Speed = 10f;

    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.ThrowingKnife}";

    private ref float KakashiIndex => ref Projectile.ai[0];
    private ref float Timer => ref Projectile.ai[1];

    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 240;
    }

    public override void AI()
    {
        Player target = Main.player[Projectile.owner];
        NPC kakashi = Main.npc[(int)KakashiIndex];
        if (!target.active || target.dead || !kakashi.active)
        {
            Projectile.Kill();
            return;
        }

        Timer++;
        if (Timer < WindupTicks)
        {
            // Held above Kakashi's head, pointing at the player, glinting.
            Projectile.Center = kakashi.Top + new Vector2(0f, -10f);
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation = (target.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2;
            if (Timer % 10 == 0)
                Dust.NewDustPerfect(Projectile.Center, DustID.SilverCoin, Vector2.Zero, 0, default, 0.8f).noGravity = true;
            return;
        }

        if (Timer == WindupTicks)
        {
            Projectile.velocity = Projectile.DirectionTo(target.Center) * Speed;
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
        }
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        if (Projectile.owner != Main.myPlayer || !Projectile.Hitbox.Intersects(target.Hitbox))
            return;

        int awayDirection = Projectile.velocity.X >= 0f ? 1 : -1;
        bool dodged = target.GetModPlayer<SubstitutionPlayer>().TryTrainingDodge(awayDirection);
        CombatText.NewText(kakashi.getRect(), Color.White, dodged ? "不错嘛。" : "太慢了。（练习不造成伤害）");
        Projectile.Kill();
    }
}
