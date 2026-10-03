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

// Substitution Jutsu as logs (specs/装备与忍术系统.spec.md; user, 2026-10-03: pressing a key just before a hit was no
// use in a boss fight, where all attention goes to moving). An enemy's hit takes a log instead of the player, with
// nothing to press: the player is swapped to one side, the log falls where they stood, and two seconds of stealth
// follow (StealthPlayer). Logs come back with time, sooner for every hit landed. The key spends a log on purpose:
// out of a bind (sand coffin, killing intent), or otherwise a blink in the direction held, into stealth. Sealed
// chakra points (Neji) stop both. Kakashi's drill still trains a press just before a hit (its own free standby).
public sealed class SubstitutionPlayer : ModPlayer
{
    private int standby;
    private int ticksSinceHint;
    private int ticksSinceActivation;
    private int logProgress;
    private int hitBonusTicks;

    public int Cooldown { get; private set; }
    public bool Mastered { get; private set; }
    public int HintsShown { get; private set; }
    public int Logs { get; private set; }
    // Extra logs from equipment, reset every tick (accessories and armour add to it in UpdateEquip).
    public int ExtraLogs { get; set; }
    public int MaxLogs => ChakraRules.StartingLogs + ExtraLogs;
    // How far the next log has come back, 0 to 1 (the HUD).
    public float NextLog => Logs >= MaxLogs ? 1f : logProgress / (float)Player.GetModPlayer<StyleCorePlayer>().LogRegenTicks;

    public override void Initialize()
    {
        standby = 0;
        Cooldown = 0;
        Mastered = false;
        HintsShown = 0;
        Logs = ChakraRules.StartingLogs;
        logProgress = 0;
        ticksSinceHint = ChakraRules.SubstitutionHintSpacingTicks;
        ticksSinceActivation = ChakraRules.PracticeEarlyWindowTicks + 1;
    }

    public override void ResetEffects() => ExtraLogs = 0;

    public override void OnRespawn()
    {
        Logs = MaxLogs;
        logProgress = 0;
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
        if (Logs > MaxLogs)
            Logs = MaxLogs;
        (Logs, logProgress) = ChakraRules.TickLogs(Logs, MaxLogs, logProgress,
            Player.GetModPlayer<StyleCorePlayer>().LogRegenTicks, hitBonusTicks);
        hitBonusTicks = 0;
    }

    // Every hit landed brings the next log a little sooner.
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!target.friendly && target.lifeMax > 5)
            hitBonusTicks = ChakraRules.LogRegenPerHitTicks;
    }

    private void TryActivate()
    {
        if (Player.dead || Player.CCed && !Player.GetModPlayer<JutsuStatusPlayer>().Bound)
            return;
        JutsuStatusPlayer status = Player.GetModPlayer<JutsuStatusPlayer>();
        if (status.SubstitutionSealed)
        {
            CombatText.NewText(Player.getRect(), new Color(170, 200, 255), "点穴：查克拉被封，结不了印");
            return;
        }

        // Kakashi's drill: a free standby, as before, to train the timing.
        if (Player.GetModPlayer<SubstitutionDrillPlayer>().Active)
        {
            if (Cooldown > 0)
                return;
            standby = ChakraRules.SubstitutionWindowTicks;
            Cooldown = ChakraRules.SubstitutionCooldownFor(true);
            ticksSinceActivation = 0;
            SoundEngine.PlaySound(SoundID.Item7, Player.Center);
            return;
        }

        ChakraPlayer chakra = Player.GetModPlayer<ChakraPlayer>();
        // Bound by sand or frozen by killing intent: a log takes the player's place at once (no chakra needed).
        int cost = status.Bound ? 0 : ChakraRules.SubstitutionCost;
        switch (ChakraRules.CheckSubstitution(chakra.Chakra, Cooldown, cost, Logs))
        {
            case ChakraRules.Activation.CoolingDown:
                return;
            case ChakraRules.Activation.NoLog:
                CombatText.NewText(Player.getRect(), new Color(200, 170, 110), "没有木头了");
                return;
            case ChakraRules.Activation.NotEnoughChakra:
                CombatText.NewText(Player.getRect(), new Color(120, 180, 255), "查克拉不足");
                return;
        }

        if (cost > 0)
            chakra.TrySpend(cost);
        Logs--;
        Cooldown = ChakraRules.SubstitutionCooldownTicks;
        Mastered = true;
        if (status.Bound)
        {
            status.Break();
            Substitute(-Player.direction, ChakraRules.SubstitutionImmuneTicks);
            return;
        }
        // A blink the way the player is heading (or facing), into stealth.
        int heading = Player.controlLeft ? -1 : Player.controlRight ? 1 : Player.direction;
        Substitute(heading, ChakraRules.BlinkImmuneTicks);
    }

    public override bool FreeDodge(Player.HurtInfo info)
    {
        if (Player.whoAmI != Main.myPlayer)
            return false;
        int away = info.HitDirection != 0 ? info.HitDirection : -Player.direction;
        // Sharingan foresight: the first hit while it lasts is substituted without spending a log.
        StyleCorePlayer styles = Player.GetModPlayer<StyleCorePlayer>();
        if (styles.ForesightTicks > 0)
        {
            styles.ForesightTook(AttackerOf(info));
            Substitute(away, ChakraRules.SubstitutionImmuneTicks);
            return true;
        }
        bool fromEnemy = info.DamageSource.SourceNPCIndex >= 0 || info.DamageSource.SourceProjectileType > 0;
        if (!ChakraRules.AutoSubstitutes(Logs, Player.GetModPlayer<JutsuStatusPlayer>().SubstitutionSealed, fromEnemy))
            return false;
        Logs--;
        Substitute(away, ChakraRules.SubstitutionImmuneTicks);
        Hint();
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
            Substitute(awayDirection, ChakraRules.SubstitutionImmuneTicks);
        return outcome;
    }

    private void Substitute(int direction, int immuneTicks)
    {
        standby = 0;

        // The log is 32 px tall; centre it so it stands where the player's feet were.
        Projectile.NewProjectile(Player.GetSource_Misc("Substitution"), Player.Bottom - new Vector2(0f, 16f), Vector2.Zero,
            ModContent.ProjectileType<SubstitutionLog>(), 0, 0f, Player.whoAmI);
        Puff(Player.Center);

        if (FindLanding(direction, out Vector2 landing))
        {
            Player.RemoveAllGrapplingHooks();
            Player.position = landing;
            Player.velocity = Vector2.Zero;
            Player.fallStart = (int)(Player.position.Y / 16f);
            Puff(Player.Center);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.PlayerControls, number: Player.whoAmI);
        }

        Player.SetImmuneTimeForAllTypes(immuneTicks);
        Player.GetModPlayer<StealthPlayer>().Grant(ChakraRules.StealthTicks);
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

    // The first few logs taken explain the key, until the player has used it.
    private void Hint()
    {
        if (!ShinobiClientConfig.Instance.ShowSubstitutionHints || !ChakraRules.ShouldShowHint(Mastered, HintsShown, ticksSinceHint))
            return;
        HintsShown++;
        ticksSinceHint = 0;
        Main.NewText($"提示：木头替你挡下了这一击（还剩 {Logs} 根，会慢慢恢复，打中敌人恢复更快）。" +
            $"按【{ShinobiKeybinds.SubstitutionKeyName()}】可以主动替身：朝移动方向瞬移并潜伏，下一击必定暴击；被沙子裹住时也靠它挣脱。",
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
