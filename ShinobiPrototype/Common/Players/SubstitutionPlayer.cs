using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Common.Players;

// Substitution Jutsu: press the key just before a hit to trade places with a log.
public sealed class SubstitutionPlayer : ModPlayer
{
    private int standby;
    private int ticksSinceHint;
    private int ticksSinceActivation;

    public int Cooldown { get; private set; }
    public bool Mastered { get; private set; }
    public int HintsShown { get; private set; }

    public override void Initialize()
    {
        standby = 0;
        Cooldown = 0;
        Mastered = false;
        HintsShown = 0;
        ticksSinceHint = ChakraRules.SubstitutionHintSpacingTicks;
        ticksSinceActivation = ChakraRules.PracticeEarlyWindowTicks + 1;
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (ShinobiKeybinds.Substitution.JustPressed)
            TryActivate();
    }

    public override void PostUpdate()
    {
        if (standby > 0)
            standby--;
        if (Cooldown > 0)
            Cooldown--;
        if (ticksSinceHint < ChakraRules.SubstitutionHintSpacingTicks)
            ticksSinceHint++;
        if (ticksSinceActivation <= ChakraRules.PracticeEarlyWindowTicks)
            ticksSinceActivation++;
    }

    private void TryActivate()
    {
        if (Player.dead || Player.CCed)
            return;
        JutsuStatusPlayer status = Player.GetModPlayer<JutsuStatusPlayer>();
        if (status.SubstitutionSealed)
        {
            CombatText.NewText(Player.getRect(), new Color(170, 200, 255), "点穴：查克拉被封，结不了印");
            return;
        }

        ChakraPlayer chakra = Player.GetModPlayer<ChakraPlayer>();
        bool drilling = Player.GetModPlayer<SubstitutionDrillPlayer>().Active;
        int cost = ChakraRules.SubstitutionCostFor(drilling);
        switch (ChakraRules.CheckSubstitution(chakra.Chakra, Cooldown, cost))
        {
            case ChakraRules.Activation.CoolingDown:
                CombatText.NewText(Player.getRect(), Color.LightGray, $"替身术冷却中 {Cooldown / 60f:0.0}s");
                return;
            case ChakraRules.Activation.NotEnoughChakra:
                CombatText.NewText(Player.getRect(), new Color(120, 180, 255), "查克拉不足");
                return;
        }

        if (cost > 0)
            chakra.TrySpend(cost);
        // The Sharingan widens the window (StyleCorePlayer).
        standby = Player.GetModPlayer<StyleCorePlayer>().SubstitutionWindowTicks;
        Cooldown = ChakraRules.SubstitutionCooldownFor(drilling);
        ticksSinceActivation = 0;
        SoundEngine.PlaySound(SoundID.Item7, Player.Center);
        for (int i = 0; i < 6; i++)
            Dust.NewDust(Player.position, Player.width, Player.height, DustID.Smoke, 0f, -1f, 120, default, 0.9f);
        // Bound by sand or frozen by killing intent: the log takes the player's place at once.
        if (status.Bound)
        {
            status.Break();
            Substitute(-Player.direction);
        }
    }

    public override bool FreeDodge(Player.HurtInfo info)
    {
        if (Player.whoAmI != Main.myPlayer)
            return false;
        // Sharingan foresight: the first hit while it lasts is substituted for free.
        StyleCorePlayer styles = Player.GetModPlayer<StyleCorePlayer>();
        if (styles.ForesightTicks > 0)
        {
            styles.ForesightTook(AttackerOf(info));
            Substitute(info.HitDirection != 0 ? info.HitDirection : -Player.direction);
            return true;
        }
        if (standby <= 0)
            return false;

        Substitute(info.HitDirection != 0 ? info.HitDirection : -Player.direction);
        return true;
    }

    // The enemy behind a hit: the NPC itself, or for a projectile the nearest enemy (the likely thrower).
    private int AttackerOf(Player.HurtInfo info)
    {
        if (info.DamageSource.SourceNPCIndex >= 0)
            return info.DamageSource.SourceNPCIndex;
        int nearest = -1;
        float best = 60f * 16f;
        foreach (NPC npc in Main.ActiveNPCs)
            if (!npc.friendly && npc.Distance(Player.Center) < best)
            {
                best = npc.Distance(Player.Center);
                nearest = npc.whoAmI;
            }
        return nearest;
    }

    // Kakashi's drill kunai deal no damage, so they ask for the dodge directly instead of going through FreeDodge.
    public ChakraRules.PracticeOutcome TakeDrillKunai(int awayDirection)
    {
        ChakraRules.PracticeOutcome outcome = ChakraRules.JudgePracticeHit(standby > 0, ticksSinceActivation);
        if (outcome == ChakraRules.PracticeOutcome.Substituted)
            Substitute(awayDirection);
        return outcome;
    }

    private void Substitute(int awayDirection)
    {
        standby = 0;
        Mastered = true;

        // The log is 32 px tall; centre it so it stands where the player's feet were.
        Projectile.NewProjectile(Player.GetSource_Misc("Substitution"), Player.Bottom - new Vector2(0f, 16f), Vector2.Zero,
            ModContent.ProjectileType<SubstitutionLog>(), 0, 0f, Player.whoAmI);
        Puff(Player.Center);

        if (FindLanding(awayDirection, out Vector2 landing))
        {
            Player.RemoveAllGrapplingHooks();
            Player.position = landing;
            Player.velocity = Vector2.Zero;
            Player.fallStart = (int)(Player.position.Y / 16f);
            Puff(Player.Center);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.PlayerControls, number: Player.whoAmI);
        }

        Player.SetImmuneTimeForAllTypes(ChakraRules.SubstitutionImmuneTicks);
        SoundEngine.PlaySound(SoundID.DoubleJump, Player.Center);
        CombatText.NewText(Player.getRect(), new Color(200, 170, 110), "替身术！");
    }

    private bool FindLanding(int awayDirection, out Vector2 landing)
    {
        foreach ((int x, int y) in ChakraRules.LandingOffsets(awayDirection))
        {
            Vector2 candidate = Player.position + new Vector2(x * 16f, y * 16f);
            if (Collision.SolidCollision(candidate, Player.width, Player.height) ||
                Collision.LavaCollision(candidate, Player.width, Player.height) ||
                !Collision.CanHitLine(Player.position, Player.width, Player.height, candidate, Player.width, Player.height))
                continue;

            landing = candidate;
            return true;
        }

        landing = Player.position;
        return false;
    }

    private void Puff(Vector2 center)
    {
        for (int i = 0; i < 18; i++)
            Dust.NewDustPerfect(center + Main.rand.NextVector2Circular(14f, 22f), DustID.Smoke,
                Main.rand.NextVector2Circular(2f, 2f), 100, default, 1.4f);
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (Player.whoAmI != Main.myPlayer)
            return;

        bool fromEnemy = info.DamageSource.SourceNPCIndex >= 0 || info.DamageSource.SourceProjectileType > 0;
        if (!fromEnemy || !ShinobiClientConfig.Instance.ShowSubstitutionHints || !ChakraRules.ShouldShowHint(Mastered, HintsShown, ticksSinceHint,
                Player.GetModPlayer<ChakraPlayer>().Chakra, Cooldown))
            return;

        HintsShown++;
        ticksSinceHint = 0;
        Main.NewText($"提示：按【{ShinobiKeybinds.SubstitutionKeyName()}】施展替身术——在受击前一刻使用，可完全闪避这次伤害。",
            255, 220, 120);
    }

    public override void SaveData(TagCompound tag)
    {
        if (Mastered)
            tag["substitutionMastered"] = true;
        if (HintsShown > 0)
            tag["substitutionHints"] = HintsShown;
    }

    public override void LoadData(TagCompound tag)
    {
        Mastered = tag.GetBool("substitutionMastered");
        HintsShown = tag.GetInt("substitutionHints");
    }
}
