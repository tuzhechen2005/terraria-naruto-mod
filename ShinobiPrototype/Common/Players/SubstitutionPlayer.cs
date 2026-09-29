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
    }

    private void TryActivate()
    {
        if (Player.dead || Player.CCed)
            return;

        ChakraPlayer chakra = Player.GetModPlayer<ChakraPlayer>();
        switch (ChakraRules.CheckSubstitution(chakra.Chakra, Cooldown))
        {
            case ChakraRules.Activation.CoolingDown:
                CombatText.NewText(Player.getRect(), Color.LightGray, $"替身术冷却中 {Cooldown / 60f:0.0}s");
                return;
            case ChakraRules.Activation.NotEnoughChakra:
                CombatText.NewText(Player.getRect(), new Color(120, 180, 255), "查克拉不足");
                return;
        }

        chakra.TrySpend(ChakraRules.SubstitutionCost);
        standby = ChakraRules.SubstitutionWindowTicks;
        Cooldown = ChakraRules.SubstitutionCooldownTicks;
        SoundEngine.PlaySound(SoundID.Item7, Player.Center);
        for (int i = 0; i < 6; i++)
            Dust.NewDust(Player.position, Player.width, Player.height, DustID.Smoke, 0f, -1f, 120, default, 0.9f);
    }

    public override bool FreeDodge(Player.HurtInfo info)
    {
        if (standby <= 0 || Player.whoAmI != Main.myPlayer)
            return false;

        standby = 0;
        Mastered = true;

        // The log is 32 px tall; centre it so it stands where the player's feet were.
        Projectile.NewProjectile(Player.GetSource_Misc("Substitution"), Player.Bottom - new Vector2(0f, 16f), Vector2.Zero,
            ModContent.ProjectileType<SubstitutionLog>(), 0, 0f, Player.whoAmI);
        Puff(Player.Center);

        int awayDirection = info.HitDirection != 0 ? info.HitDirection : -Player.direction;
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
        return true;
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
        if (!fromEnemy || !ChakraRules.ShouldShowHint(Mastered, HintsShown, ticksSinceHint,
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
