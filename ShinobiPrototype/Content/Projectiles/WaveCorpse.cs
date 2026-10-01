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
// fight's "is a boss still here" checks are unaffected. During the epilogue the one who fell last says their lines,
// then gets up, staggers over at a slow fixed pace and collapses at the other's side (WaveEpilogueRules). Art: wave-corpses-v1 (Zabuza_Lying /
// Haku_Lying) and wave-epilogue-walk-v1 (Rise / Stagger / Collapse); until those frames arrive, the kneeling and
// unarmed frames stand in.
public sealed class WaveCorpse : ModProjectile
{
    public const int Zabuza = 0;
    public const int Haku = 1;

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
            int t = WaveEpilogueSystem.Tick;
            if (IsWalker && Find(1 - Who) is Projectile other)
            {
                float gap = other.Center.X - Projectile.Center.X;
                // Face the other from the moment they start to rise.
                if (WaveEpilogueSystem.Phase != WaveEpilogueRules.WalkerPhase.Lying)
                    Projectile.ai[1] = Math.Sign(gap) == 0 ? Projectile.ai[1] : Math.Sign(gap);
                if (WaveEpilogueSystem.Phase == WaveEpilogueRules.WalkerPhase.Walking &&
                    Math.Abs(gap) > WaveEpilogueSystem.RestBesideGap)
                    Projectile.velocity.X = Math.Sign(gap) * WaveEpilogueRules.WalkSpeed;
            }
            if (WaveEpilogueSystem.Fading)
                Projectile.alpha = Math.Min(255, Projectile.alpha + 3);
        }
        else if (sawEpilogue)
            Projectile.Kill();
    }

    private bool IsWalker => WaveEpilogueSystem.Playing && Who == (WaveEpilogueSystem.HakuFellFirst ? Zabuza : Haku);

    public override bool OnTileCollide(Vector2 oldVelocity) => false;

    // Standing, rising or collapsing: an upright frame drawn on the body's spot. Lying and down: the lying art.
    private bool DrawUpright(Color color)
    {
        if (!IsWalker)
            return false;
        int t = WaveEpilogueSystem.Tick;
        WaveEpilogueRules.WalkerPhase phase = WaveEpilogueSystem.Phase;
        int rise = WaveEpilogueRules.RiseFrom;
        int collapse = WaveEpilogueRules.CollapseFrom(WaveEpilogueSystem.HakuFellFirst, WaveEpilogueSystem.WalkTicks);
        string prefix = Who == Zabuza ? "Zabuza" : "Haku";
        BossSprites.Canvas canvas = Who == Zabuza ? BossSprites.Zabuza : BossSprites.Haku;
        (string action, int frame, int frames, string fallback, int fallbackFrames) = phase switch
        {
            WaveEpilogueRules.WalkerPhase.Rising => ("Rise", (t - rise) * 3 / WaveEpilogueRules.RiseTicks, 3,
                Who == Zabuza ? "Kneel" : "Idle", 1),
            WaveEpilogueRules.WalkerPhase.Walking => ("Stagger", t / 16, 4, Who == Zabuza ? "Unarmed" : "Idle", Who == Zabuza ? 6 : 4),
            // At the other's side, waiting for the last words: on their feet, swaying.
            WaveEpilogueRules.WalkerPhase.Standing => ("Rise", 2, 3, Who == Zabuza ? "Kneel" : "Idle", 1),
            WaveEpilogueRules.WalkerPhase.Collapsing => ("Collapse", (t - collapse) * 3 / WaveEpilogueRules.CollapseTicks, 3,
                Who == Zabuza ? "Kneel" : "Idle", 1),
            _ => (null, 0, 0, null, 0),
        };
        if (action == null)
            return false;
        // The last collapse frame hands over to the lying art.
        if (phase == WaveEpilogueRules.WalkerPhase.Collapsing && frame >= 2 && !BossSprites.Has($"{prefix}_Collapse_2"))
            return false;
        Vector2 feet = Projectile.Bottom;
        if (BossSprites.TryDraw(Main.spriteBatch, prefix, action, frame, frames, canvas, feet, Facing, color, Main.screenPosition))
            return true;
        return BossSprites.TryDraw(Main.spriteBatch, prefix, fallback, Who == Zabuza && fallback == "Unarmed" ? t / 12 : 0,
            fallbackFrames, canvas, feet, Facing, color, Main.screenPosition);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Color color = BossSprites.Lit(lightColor, 0.35f) * Projectile.Opacity;
        if (DrawUpright(color))
            return false;
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
