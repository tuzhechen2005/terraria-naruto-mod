using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Content.Projectiles;

// A fallen Zabuza or Haku, lying where they dropped until the epilogue ends (user, 2026-09-29). Not an NPC, so the
// fight's "is a boss still here" checks are unaffected. During the epilogue the one who fell last drags themself
// next to the other. Art: wave-corpses-v1 (Zabuza_Lying / Haku_Lying); until then a boss frame turned on its side.
public sealed class WaveCorpse : ModProjectile
{
    public const int Zabuza = 0;
    public const int Haku = 1;
    private const float CrawlSpeed = 0.5f;
    private const float RestBesideGap = 44f;

    private bool sawEpilogue;

    public override string Texture => "ShinobiPrototype/Content/NPCs/Zabuza_Idle_0";

    private int Who => (int)Projectile.ai[0];
    private int Facing => Projectile.ai[1] >= 0f ? 1 : -1;

    public override void SetDefaults()
    {
        Projectile.width = 60;
        Projectile.height = 14;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.penetrate = -1;
        Projectile.tileCollide = true;
        Projectile.timeLeft = 36000;
        Projectile.netImportant = true;
        Projectile.aiStyle = -1;
    }

    public static Projectile Find(int who)
    {
        int type = ModContent.ProjectileType<WaveCorpse>();
        foreach (Projectile p in Main.ActiveProjectiles)
            if (p.type == type && (int)p.ai[0] == who)
                return p;
        return null;
    }

    public override void AI()
    {
        Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.3f, 10f);
        Projectile.velocity.X = 0f;

        if (WaveEpilogueSystem.Playing)
        {
            sawEpilogue = true;
            int crawler = WaveEpilogueSystem.HakuFellFirst ? Zabuza : Haku;
            int t = WaveEpilogueSystem.Tick;
            if (Who == crawler && t >= WaveEpilogueRules.CrawlFrom && t <= WaveEpilogueRules.CrawlTo &&
                Find(1 - Who) is Projectile other)
            {
                float gap = other.Center.X - Projectile.Center.X;
                if (Math.Abs(gap) > RestBesideGap)
                    Projectile.velocity.X = Math.Sign(gap) * CrawlSpeed;
            }
            if (WaveEpilogueSystem.Fading)
                Projectile.alpha = Math.Min(255, Projectile.alpha + 3);
        }
        else if (sawEpilogue)
            Projectile.Kill();
    }

    public override bool OnTileCollide(Vector2 oldVelocity) => false;

    public override bool PreDraw(ref Color lightColor)
    {
        Color color = BossSprites.Lit(lightColor, 0.35f) * Projectile.Opacity;
        string lying = Who == Zabuza ? "ShinobiPrototype/Content/NPCs/Zabuza_Lying" : "ShinobiPrototype/Content/NPCs/Haku_Lying";
        SpriteEffects flip = Facing >= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        if (ModContent.HasAsset(lying))
        {
            Texture2D art = ModContent.Request<Texture2D>(lying).Value;
            Vector2 bottom = Projectile.Bottom - Main.screenPosition;
            Main.EntitySpriteDraw(art, bottom, null, color, 0f, new Vector2(art.Width / 2f, art.Height), 1f, flip);
            return false;
        }

        // Placeholder: the idle frame turned on its side, head towards the facing direction.
        string frame = Who == Zabuza ? "ShinobiPrototype/Content/NPCs/Zabuza_Idle_0" : "ShinobiPrototype/Content/NPCs/Haku_Idle_0";
        BossSprites.Canvas canvas = Who == Zabuza ? BossSprites.Zabuza : BossSprites.Haku;
        Texture2D texture = ModContent.Request<Texture2D>(frame).Value;
        int bodyHeight = Who == Zabuza ? ZabuzaCombatRules.BodyHeight : WaveDuoRules.HakuBodyHeight;
        Vector2 pivot = new(canvas.CenterX, canvas.BaselineY - bodyHeight / 2f);
        Vector2 at = Projectile.Bottom - new Vector2(0f, bodyHeight * 0.25f) - Main.screenPosition;
        Main.EntitySpriteDraw(texture, at, null, color, Facing * MathHelper.PiOver2, pivot, 1f, flip);
        return false;
    }
}
