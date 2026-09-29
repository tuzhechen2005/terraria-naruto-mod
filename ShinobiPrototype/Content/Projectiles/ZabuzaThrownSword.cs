using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Content.Projectiles;

// Zabuza's frenzy throw: the blade spins out along a line, then homes back to his hand.
// ai[0] = owner whoAmI, ai[1] = 0 outbound / 1 returning, localAI[0] = distance traveled.
public sealed class ZabuzaThrownSword : ModProjectile
{
    public override string Texture => "ShinobiPrototype/Content/NPCs/ZabuzaSlashV2";

    public override void SetDefaults()
    {
        Projectile.width = ZabuzaCombatRules.SwordWidth;
        Projectile.height = ZabuzaCombatRules.SwordHeight;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = ZabuzaCombatRules.SwordThrowMaxTicks + 90;
    }

    public override void AI()
    {
        int ownerIndex = (int)Projectile.ai[0];
        if (ownerIndex < 0 || ownerIndex >= Main.maxNPCs || !Main.npc[ownerIndex].active ||
            Main.npc[ownerIndex].type != ModContent.NPCType<ZabuzaBoss>())
        {
            Projectile.Kill();
            return;
        }
        NPC owner = Main.npc[ownerIndex];
        Projectile.localAI[1]++;
        if (Projectile.ai[1] == 0f)
        {
            Projectile.localAI[0] += Projectile.velocity.Length();
            if (ZabuzaCombatRules.SwordShouldReturn(Projectile.localAI[0], (int)Projectile.localAI[1]))
            {
                Projectile.ai[1] = 1f;
                Projectile.netUpdate = true;
            }
        }
        else
        {
            Vector2 toOwner = owner.Center - Projectile.Center;
            if (toOwner.Length() < 40f)
            {
                Projectile.Kill();
                return;
            }
            float speed = ZabuzaCombatRules.SwordThrowSpeed * 1.1f;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(toOwner) * speed, 0.12f);
        }
        Projectile.rotation += 0.45f * Math.Sign(Projectile.velocity.X == 0f ? 1f : Projectile.velocity.X);
        if (Projectile.localAI[1] % 12 == 1)
            SoundEngine.PlaySound(SoundID.Item7, Projectile.Center);
        Lighting.AddLight(Projectile.Center, 0.45f, 0.2f, 0.6f);
        if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(2))
            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20f, 20f),
                DustID.Shadowflame, -Projectile.velocity * 0.1f, 60,
                new Color(215, 125, 255), 1.1f).noGravity = true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        string art = "ShinobiPrototype/Content/Projectiles/ZabuzaThrownSword";
        Texture2D texture = ModContent.HasAsset(art)
            ? ModContent.Request<Texture2D>(art).Value
            : null;
        Vector2 center = Projectile.Center - Main.screenPosition;
        center = new Vector2((float)Math.Round(center.X), (float)Math.Round(center.Y));
        Color color = Color.Lerp(lightColor, Color.White, 0.6f);
        if (texture != null)
        {
            Main.spriteBatch.Draw(texture, center, null, color, Projectile.rotation,
                texture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            return false;
        }
        // Placeholder until the dedicated blade sprite arrives: a spinning steel bar.
        Texture2D pixel = Terraria.GameContent.TextureAssets.MagicPixel.Value;
        Main.spriteBatch.Draw(pixel, center, new Rectangle(0, 0, 1, 1), new Color(200, 210, 220),
            Projectile.rotation, new Vector2(0.5f, 0.5f), new Vector2(64f, 14f), SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(pixel, center, new Rectangle(0, 0, 1, 1), new Color(70, 60, 80),
            Projectile.rotation, new Vector2(0.5f, 0.5f), new Vector2(64f, 3f), SpriteEffects.None, 0f);
        return false;
    }
}
