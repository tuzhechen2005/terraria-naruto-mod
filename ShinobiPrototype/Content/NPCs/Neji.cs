using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.NPCs;

// Hyūga Neji, the optional sparring bout at the stadium after the finals begin (specs/M2_中忍考试篇.spec.md 3.5):
// Gentle Fist palms seal chakra points (less maximum chakra; three seals stop substitution), the Rotation throws back
// whatever the player shot at him, and the Sixty-Four Palms seal everything inside the trigram ring.
[AutoloadBossHead]
public sealed class Neji : ExamBoss
{
    private const float Approach = 0f;
    private const float Palms = 1f;
    private const float RotationWindup = 2f;
    private const float Rotation = 3f;
    private const float SixtyFourWarn = 4f;
    private const float Recovery = 5f;

    protected override Color Tint => new(235, 225, 245);
    protected override int LifeMax => ExamBossRules.NejiLife;
    protected override int Defense => ExamBossRules.NejiDefense;

    protected override string SpritePrefix => "Neji";

    protected override (string Action, int Frames, int TicksPerFrame, bool Loop) Pose => State switch
    {
        Palms => ("Palm", 3, 4, true),
        RotationWindup or Rotation => ("Rotation", 4, 4, true),
        SixtyFourWarn => ("SixtyFour_Windup", 2, 12, true),
        Recovery when Timer < 24f && lastWasSixtyFour => ("SixtyFour_Strike", 3, 4, true),
        _ => Moving("Run", "Idle"),
    };

    private bool lastWasSixtyFour;

    protected override void Fight(Player target)
    {
        float distance = Math.Abs(target.Center.X - NPC.Center.X);
        if (Timer == 1f && State == Approach && NPC.localAI[0] == 0f)
        {
            NPC.localAI[0] = 1f;
            Say("……来吧。让你看看，什么叫做命运。", new Color(200, 220, 255));
        }
        switch (State)
        {
            case Approach:
                RunTo(target.Center.X, 4.2f, target);
                if (Deciding && Timer > 40f)
                {
                    // Shot at from range: the Rotation; close: the palms; now and then the Sixty-Four.
                    if (PlayerShotsNear(260f) && Main.rand.NextBool(2))
                        Enter(RotationWindup);
                    else if (distance < 5 * 16)
                        Enter(Main.rand.NextBool(4) ? SixtyFourWarn : Palms);
                    else if (Timer > 150f)
                        Enter(Main.rand.NextBool(3) ? SixtyFourWarn : RotationWindup);
                }
                break;

            case Palms:
                Face(target.Center.X);
                NPC.velocity.X = NPC.direction * 2.5f;
                // Three palms; the first only after the stance has shown (specs/敌方伤害标准.spec.md: 15 ticks, was 6).
                if ((int)Timer >= EnemyDamageRules.Neji.FirstPalmTick && ((int)Timer - EnemyDamageRules.Neji.FirstPalmTick) % EnemyDamageRules.Neji.PalmSpacingTicks == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                    if (Deciding)
                        JutsuHitbox.Spawn(NPC, JutsuKind.GentleFist, NPC.Center + new Vector2(NPC.direction * 26f, 0f),
                            new Vector2(NPC.direction * 2.5f, 0f), 34, 40, EnemyDamage.Projectile(EnemyDamageRules.Neji.Palm));
                }
                if (Timer >= 40f)
                    Enter(Recovery);
                break;

            case RotationWindup:
                NPC.velocity.X *= 0.7f;
                Telegraph(DustID.IceTorch, 40f);
                if (Timer >= EnemyDamageRules.Neji.RotationWindupTicks)
                {
                    Say("八卦掌·回天！", new Color(190, 220, 255));
                    SoundEngine.PlaySound(SoundID.Item60, NPC.Center);
                    if (Deciding)
                        JutsuHitbox.Spawn(NPC, JutsuKind.Rotation, NPC.Center, Vector2.Zero,
                            ExamBossRules.RotationRadiusPx * 2, ExamBossRules.RotationRadiusPx * 2, EnemyDamage.Projectile(EnemyDamageRules.Neji.Rotation), 10f);
                    Enter(Rotation);
                }
                break;

            case Rotation:
                NPC.velocity.X = 0f;
                DeflectShots();
                if (Timer >= ExamBossRules.RotationTicks)
                    Enter(Recovery);
                break;

            case SixtyFourWarn:
                NPC.velocity.X *= 0.6f;
                if (Timer == 1f)
                    Say("你已在我八卦的范围之内。", new Color(190, 220, 255));
                if (Main.netMode != NetmodeID.Server)
                    for (int i = 0; i < 4; i++)
                        Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2CircularEdge(ExamBossRules.SixtyFourRadiusPx,
                            ExamBossRules.SixtyFourRadiusPx), DustID.IceTorch, Vector2.Zero, 0, default, 1.3f).noGravity = true;
                if (Timer >= ExamBossRules.SixtyFourWarnTicks)
                {
                    Say("八卦六十四掌！", new Color(190, 220, 255));
                    SoundEngine.PlaySound(SoundID.Item71, NPC.Center);
                    lastWasSixtyFour = true;
                    if (Deciding)
                        JutsuHitbox.Spawn(NPC, JutsuKind.SixtyFour, NPC.Center, Vector2.Zero,
                            ExamBossRules.SixtyFourRadiusPx * 2, ExamBossRules.SixtyFourRadiusPx * 2, EnemyDamage.Projectile(EnemyDamageRules.Neji.SixtyFour));
                    Enter(Recovery);
                }
                break;

            case Recovery:
                NPC.velocity.X *= 0.85f;
                if (Timer >= 45f)
                {
                    lastWasSixtyFour = false;
                    Enter(Approach);
                }
                break;
        }
    }

    private bool PlayerShotsNear(float range)
    {
        foreach (Projectile projectile in Main.ActiveProjectiles)
            if (projectile.friendly && !projectile.minion && projectile.damage > 0 &&
                Vector2.Distance(projectile.Center, NPC.Center) < range)
                return true;
        return false;
    }

    // Each client puts out its own player's shots that reach the dome (projectiles belong to their owner's client).
    private void DeflectShots()
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        foreach (Projectile projectile in Main.ActiveProjectiles)
        {
            if (projectile.owner != Main.myPlayer || !projectile.friendly || projectile.minion ||
                Vector2.Distance(projectile.Center, NPC.Center) > ExamBossRules.RotationRadiusPx + 8)
                continue;
            for (int i = 0; i < 6; i++)
                Dust.NewDustPerfect(projectile.Center, DustID.IceTorch, Main.rand.NextVector2Circular(3f, 3f), 0, default, 1.2f)
                    .noGravity = true;
            projectile.Kill();
        }
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) =>
        npcLoot.Add(Terraria.GameContent.ItemDropRules.ItemDropRule.Common(ModContent.ItemType<Items.StyleCores.ByakuganCore>(), 4));

    public override void OnKill()
    {
        StoryWorld.DownedNeji = true;
        Tell("宁次：……命运，并不是早就注定的吗。你让我……看到了不一样的东西。（切磋书可以再用，随时奉陪。）", new Color(200, 220, 255));
        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.WorldData);
    }
}
