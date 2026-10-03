using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Players;

// Stealth (specs/装备与忍术系统.spec.md): for five seconds after a blink (two logs) the player is hard to see, and the
// next hit lands as a stealth strike: always critical, harder, with a burst of smoke. It is a buff (StealthBuff). Minions and sentries do not use it.
// Ninja tools get their own stealth strikes later; this is the shared part every weapon gets.
public sealed class StealthPlayer : ModPlayer
{
    private static int BuffType => ModContent.BuffType<Content.Buffs.StealthBuff>();

    public bool Hidden => Player.HasBuff(BuffType);

    public void Grant(int ticks) => Player.AddBuff(BuffType, ticks);

    public override void PostUpdate()
    {
        if (Hidden && Main.netMode != NetmodeID.Server && Main.rand.NextBool(4))
            Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2Circular(12f, 20f), DustID.Smoke,
                new Vector2(0f, -0.6f), 150, default, 0.9f).noGravity = true;
    }
    private static bool Strikes(Projectile projectile) =>
        !projectile.minion && !projectile.sentry && !ProjectileID.Sets.MinionShot[projectile.type] &&
        !ProjectileID.Sets.SentryShot[projectile.type];

    public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers) => Strike(ref modifiers);

    public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Strikes(proj))
            Strike(ref modifiers);
    }

    private void Strike(ref NPC.HitModifiers modifiers)
    {
        if (!Hidden)
            return;
        modifiers.SetCrit();
        modifiers.FinalDamage *= 1f + ChakraRules.StealthDamageBonus;
    }

    public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone) => Spend(target);

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Strikes(proj))
            Spend(target);
    }

    // The stealth strike lands: smoke bursts round the target and the player steps out of hiding.
    private void Spend(NPC target)
    {
        if (!Hidden)
            return;
        Player.ClearBuff(BuffType);
        if (Main.netMode == NetmodeID.Server)
            return;
        SoundEngine.PlaySound(SoundID.Item71 with { Pitch = 0.3f }, target.Center);
        for (int i = 0; i < 24; i++)
        {
            Vector2 out_ = Main.rand.NextVector2CircularEdge(1f, 1f);
            Dust.NewDustPerfect(target.Center + out_ * 10f, DustID.Smoke, out_ * Main.rand.NextFloat(2f, 5f), 120, default, 1.6f)
                .noGravity = true;
        }
        for (int i = 0; i < 10; i++)
            Dust.NewDustPerfect(target.Center, DustID.GoldFlame, Main.rand.NextVector2Circular(4f, 4f), 0, default, 1.2f)
                .noGravity = true;
    }

    // Hard to see while hidden.
    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        if (!Hidden)
            return;
        float a = 0.45f;
        drawInfo.colorHair *= a;
        drawInfo.colorEyes *= a;
        drawInfo.colorHead *= a;
        drawInfo.colorBodySkin *= a;
        drawInfo.colorLegs *= a;
        drawInfo.colorShirt *= a;
        drawInfo.colorUnderShirt *= a;
        drawInfo.colorPants *= a;
        drawInfo.colorShoes *= a;
        drawInfo.colorArmorHead *= a;
        drawInfo.colorArmorBody *= a;
        drawInfo.colorArmorLegs *= a;
    }
}
