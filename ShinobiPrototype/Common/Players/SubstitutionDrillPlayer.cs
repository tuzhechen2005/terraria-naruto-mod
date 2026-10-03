using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ShinobiPrototype.Content.NPCs;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Common.Players;

// Kakashi's substitution drill: he throws kunai at the player one at a time, each after an unpredictable aim.
// He only starts a throw when the player is in range and the jutsu is off cooldown. Runs on the local client
// only; the kunai never deal damage.
public sealed class SubstitutionDrillPlayer : ModPlayer
{
    private const int PromptIntervalTicks = 120;

    private int kakashi = -1;
    private int thrown;
    private int substituted;
    private int gap;
    private int promptCooldown;
    private bool kunaiInFlight;

    public bool Active => kakashi >= 0;

    public override void Initialize() => kakashi = -1;

    public void Start(int kakashiIndex)
    {
        if (Active)
            Stop();

        kakashi = kakashiIndex;
        thrown = 0;
        substituted = 0;
        kunaiInFlight = false;
        gap = ChakraRules.PracticeGapMinTicks;
        promptCooldown = 0;
        Mentor.DrillTarget = Player.whoAmI;
        Main.NewText($"卡卡西：我会朝你扔 {ChakraRules.PracticeThrows} 支苦无，瞄准的时间每次都不一样。" +
                     $"站到离我 {ChakraRules.PracticeMinRangeTiles}–{ChakraRules.PracticeMaxRangeTiles} 格的地方，" +
                     "盯住苦无，在它快到身上时按替身键。练习用的苦无不会让木头自己替你挨，全看你的时机；练习期间不耗木头和查克拉。",
            255, 220, 120);
    }

    private Kakashi Mentor => (Kakashi)Main.npc[kakashi].ModNPC;

    public override void PostUpdate()
    {
        if (!Active || Player.whoAmI != Main.myPlayer)
            return;

        NPC mentor = Main.npc[kakashi];
        if (!mentor.active || mentor.type != ModContent.NPCType<Kakashi>())
        {
            kakashi = -1;
            return;
        }
        if (Player.dead || Player.Distance(mentor.Center) > ChakraRules.PracticeLeashTiles * 16f)
        {
            Stop();
            Main.NewText("卡卡西：跑那么远，练习就先到这里吧。", 255, 220, 120);
            return;
        }

        if (promptCooldown > 0)
            promptCooldown--;
        if (kunaiInFlight)
            return;
        if (thrown >= ChakraRules.PracticeThrows)
        {
            Finish(mentor);
            return;
        }

        int cooldown = Player.GetModPlayer<SubstitutionPlayer>().Cooldown;
        switch (ChakraRules.CheckPracticeThrow(Player.Distance(mentor.Center) / 16f, cooldown))
        {
            case ChakraRules.PracticeReadiness.TooClose:
                Prompt(mentor, $"离远一点（{ChakraRules.PracticeMinRangeTiles} 格以上）");
                return;
            case ChakraRules.PracticeReadiness.TooFar:
                Prompt(mentor, "再靠近一点");
                return;
            case ChakraRules.PracticeReadiness.CoolingDown:
                return;
        }

        if (--gap <= 0)
            Throw(mentor);
    }

    private void Prompt(NPC mentor, string text)
    {
        if (promptCooldown > 0)
            return;
        promptCooldown = PromptIntervalTicks;
        CombatText.NewText(mentor.getRect(), Color.White, text);
    }

    private void Throw(NPC mentor)
    {
        int windup = Main.rand.Next(ChakraRules.PracticeWindupMinTicks, ChakraRules.PracticeWindupMaxTicks + 1);
        Projectile.NewProjectile(Player.GetSource_Misc("SubstitutionDrill"), Mentor.DrillHand, Vector2.Zero,
            ModContent.ProjectileType<KakashiPracticeKunai>(), 0, 0f, Player.whoAmI, windup, 0f, kakashi);
        kunaiInFlight = true;
        thrown++;
    }

    public void Resolve(ChakraRules.PracticeOutcome outcome)
    {
        if (!Active)
            return;

        kunaiInFlight = false;
        gap = Main.rand.Next(ChakraRules.PracticeGapMinTicks, ChakraRules.PracticeGapMaxTicks + 1);
        (string text, Color color) = outcome switch
        {
            ChakraRules.PracticeOutcome.Substituted => ("替身成功！", new Color(255, 215, 120)),
            ChakraRules.PracticeOutcome.TooEarly => ("早了！", new Color(150, 200, 255)),
            ChakraRules.PracticeOutcome.TooLate => ("晚了！", new Color(255, 130, 120)),
            _ => ("躲开了——这次试试用替身术", Color.LightGray),
        };
        if (outcome == ChakraRules.PracticeOutcome.Substituted)
            substituted++;
        CombatText.NewText(Player.getRect(), color, $"{text}（{thrown}/{ChakraRules.PracticeThrows}）");
    }

    private void Finish(NPC mentor)
    {
        string verdict = ChakraRules.PracticeVerdict(substituted, thrown);
        Stop();
        Main.NewText($"卡卡西：{thrown} 支里替身成功 {substituted} 支。{verdict}", 255, 220, 120);
        CombatText.NewText(mentor.getRect(), Color.White, $"{substituted}/{thrown}");
    }

    private void Stop()
    {
        if (kakashi >= 0 && Main.npc[kakashi].active && Main.npc[kakashi].ModNPC is Kakashi mentor)
        {
            mentor.DrillTarget = -1;
            mentor.DrillAiming = false;
        }
        kakashi = -1;
    }
}
