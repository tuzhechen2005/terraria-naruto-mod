using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;

namespace ShinobiPrototype.Content.Projectiles;

// A drill kunai from one of Kakashi's hidden shadow clones: it glints in place for a random moment, then flies
// at the player. It never deals damage; reaching the player is judged by SubstitutionPlayer.TakeDrillKunai.
public sealed class KakashiPracticeKunai : ModProjectile
{
    private const int MaxFlightTicks = 90;

    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.ThrowingKnife}";

    private ref float WindupTicks => ref Projectile.ai[0];
    private ref float Timer => ref Projectile.ai[1];
    private bool resolved;

    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 300;
    }

    public override void AI()
    {
        Player target = Main.player[Projectile.owner];
        if (!target.active || target.dead)
        {
            Projectile.Kill();
            return;
        }

        Timer++;
        Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.6f);
        if (Timer < WindupTicks)
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation = (target.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2;
            if (Timer == 1)
                SoundEngine.PlaySound(SoundID.Item35 with { Pitch = 0.6f, Volume = 0.6f }, Projectile.Center);
            if (Timer % 5 == 0)
                Dust.NewDustPerfect(Projectile.Center, DustID.SilverCoin, Main.rand.NextVector2Circular(1f, 1f), 0, default, 1.1f)
                    .noGravity = true;
            return;
        }

        if (Timer == WindupTicks)
        {
            Projectile.velocity = Projectile.DirectionTo(target.Center) * ChakraRules.PracticeKunaiSpeed;
            SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
        }
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        if (Projectile.owner != Main.myPlayer)
            return;
        if (Projectile.Hitbox.Intersects(target.Hitbox))
        {
            int awayDirection = Projectile.velocity.X >= 0f ? 1 : -1;
            Resolve(target.GetModPlayer<SubstitutionPlayer>().TakeDrillKunai(awayDirection));
        }
        else if (Timer - WindupTicks > MaxFlightTicks)
            Resolve(ChakraRules.PracticeOutcome.Evaded);
    }

    private void Resolve(ChakraRules.PracticeOutcome outcome)
    {
        resolved = true;
        Main.player[Projectile.owner].GetModPlayer<SubstitutionDrillPlayer>().Resolve(outcome);
        Projectile.Kill();
    }

    public override void OnKill(int timeLeft)
    {
        // Killed some other way (owner died, projectile cap): release the drill so it does not wait forever.
        if (!resolved && Projectile.owner == Main.myPlayer)
            Main.player[Projectile.owner].GetModPlayer<SubstitutionDrillPlayer>().Resolve(ChakraRules.PracticeOutcome.Evaded);
    }
}
