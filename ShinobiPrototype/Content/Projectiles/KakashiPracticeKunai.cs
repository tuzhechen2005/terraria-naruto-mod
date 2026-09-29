using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Content.Projectiles;

// A drill kunai in Kakashi's hand: aimed for a random moment (and until the player's jutsu is ready), then thrown.
// Drawn large with a glow and trail so it reads at a distance. It never deals damage; reaching the player is
// judged by SubstitutionPlayer.TakeDrillKunai.
public sealed class KakashiPracticeKunai : ModProjectile
{
    private const int MaxFlightTicks = 150;
    private const float DrawScale = 2f;

    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.ThrowingKnife}";

    private ref float WindupTicks => ref Projectile.ai[0];
    private ref float Timer => ref Projectile.ai[1];
    private int KakashiIndex => (int)Projectile.ai[2];
    private bool launched;
    private int flightTicks;
    private bool resolved;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 1200;
    }

    public override void AI()
    {
        Player target = Main.player[Projectile.owner];
        NPC npc = Main.npc[KakashiIndex];
        if (!target.active || target.dead || !npc.active || npc.ModNPC is not Kakashi kakashi)
        {
            Projectile.Kill();
            return;
        }

        Lighting.AddLight(Projectile.Center, 0.7f, 0.7f, 0.8f);
        if (!launched)
        {
            Aim(target, kakashi);
            return;
        }

        flightTicks++;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        if (Projectile.owner != Main.myPlayer)
            return;
        if (Projectile.Hitbox.Intersects(target.Hitbox))
        {
            int awayDirection = Projectile.velocity.X >= 0f ? 1 : -1;
            Resolve(target.GetModPlayer<SubstitutionPlayer>().TakeDrillKunai(awayDirection));
        }
        else if (flightTicks > MaxFlightTicks)
            Resolve(ChakraRules.PracticeOutcome.Evaded);
    }

    // Held raised in Kakashi's hand, pointing at the player and glinting, until the windup has passed
    // and the player's jutsu is ready.
    private void Aim(Player target, Kakashi kakashi)
    {
        Timer++;
        kakashi.DrillAiming = true;
        Projectile.Center = kakashi.DrillHand;
        Projectile.velocity = Vector2.Zero;
        Projectile.rotation = (target.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2;
        if (Timer == 1)
            SoundEngine.PlaySound(SoundID.Item35 with { Pitch = 0.6f, Volume = 0.6f }, Projectile.Center);
        if (Timer % 5 == 0)
            Dust.NewDustPerfect(Projectile.Center, DustID.SilverCoin, Main.rand.NextVector2Circular(1f, 1f), 0, default, 1.3f)
                .noGravity = true;

        int cooldown = target.GetModPlayer<SubstitutionPlayer>().Cooldown;
        if (!ChakraRules.MayReleasePracticeKunai((int)Timer, (int)WindupTicks, cooldown))
            return;

        launched = true;
        kakashi.DrillAiming = false;
        kakashi.DrillReleaseTicks = 16;
        Projectile.velocity = Projectile.DirectionTo(target.Center) * ChakraRules.PracticeKunaiSpeed;
        SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
    }

    private void Resolve(ChakraRules.PracticeOutcome outcome)
    {
        resolved = true;
        Main.player[Projectile.owner].GetModPlayer<SubstitutionDrillPlayer>().Resolve(outcome);
        Projectile.Kill();
    }

    public override void OnKill(int timeLeft)
    {
        if (Main.npc[KakashiIndex].ModNPC is Kakashi kakashi)
            kakashi.DrillAiming = false;
        // Killed some other way (owner died, projectile cap): release the drill so it does not wait forever.
        if (!resolved && Projectile.owner == Main.myPlayer)
            Main.player[Projectile.owner].GetModPlayer<SubstitutionDrillPlayer>().Resolve(ChakraRules.PracticeOutcome.Evaded);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 origin = texture.Size() / 2f;
        Vector2 half = Projectile.Size / 2f;

        if (launched)
            for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
            {
                float fade = 1f - (i + 1f) / (Projectile.oldPos.Length + 1f);
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + half - Main.screenPosition, null,
                    Color.White * 0.35f * fade, Projectile.oldRot[i], origin, DrawScale * (0.7f + 0.3f * fade),
                    SpriteEffects.None);
            }

        // A pulsing white halo behind the blade, then the blade itself at full brightness.
        float pulse = 0.5f + 0.5f * (float)System.Math.Sin(Main.GameUpdateCount * 0.3f);
        Vector2 at = Projectile.Center - Main.screenPosition;
        for (int i = 0; i < 4; i++)
        {
            Vector2 offset = new Vector2(2f, 0f).RotatedBy(MathHelper.PiOver2 * i);
            Main.EntitySpriteDraw(texture, at + offset, null, Color.White * (0.35f + 0.25f * pulse),
                Projectile.rotation, origin, DrawScale, SpriteEffects.None);
        }
        Main.EntitySpriteDraw(texture, at, null, Color.White, Projectile.rotation, origin, DrawScale, SpriteEffects.None);
        return false;
    }
}
