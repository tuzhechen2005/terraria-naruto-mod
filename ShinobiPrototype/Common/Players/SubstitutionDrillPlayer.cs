using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ShinobiPrototype.Content.NPCs;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Common.Players;

// Kakashi's substitution drill: his shadow clones throw kunai from hiding, one at a time, from either side and after
// an unpredictable pause. The drill runs only on the local client; the kunai never deal damage.
public sealed class SubstitutionDrillPlayer : ModPlayer
{
    private int kakashi = -1;
    private int thrown;
    private int substituted;
    private int gap;
    private bool kunaiInFlight;

    public bool Active => kakashi >= 0;

    public override void Initialize() => kakashi = -1;

    public void Start(int kakashiIndex)
    {
        kakashi = kakashiIndex;
        thrown = 0;
        substituted = 0;
        kunaiInFlight = false;
        gap = ChakraRules.PracticeGapMaxTicks;
        Main.NewText($"卡卡西：我的影分身会从暗处扔 {ChakraRules.PracticeThrows} 支苦无，左右都有，时机不定。" +
                     "看到寒光就盯住它，在苦无快到身上时按替身术。练习期间替身术不耗查克拉，冷却也很短。", 255, 220, 120);
    }

    public override void PostUpdate()
    {
        if (!Active || Player.whoAmI != Main.myPlayer)
            return;

        NPC mentor = Main.npc[kakashi];
        if (Player.dead || !mentor.active || mentor.type != ModContent.NPCType<Kakashi>() ||
            Player.Distance(mentor.Center) > ChakraRules.PracticeLeashTiles * 16f)
        {
            kakashi = -1;
            Main.NewText("卡卡西：跑那么远，练习就先到这里吧。", 255, 220, 120);
            return;
        }

        if (kunaiInFlight)
            return;
        if (thrown >= ChakraRules.PracticeThrows)
        {
            Finish(mentor);
            return;
        }
        if (--gap <= 0)
            Throw();
    }

    private void Throw()
    {
        int side = Main.rand.NextBool() ? 1 : -1;
        Vector2 spot = PickSpot(side);
        if (!Collision.CanHitLine(spot, 1, 1, Player.Center, 1, 1))
        {
            Vector2 other = PickSpot(-side);
            if (Collision.CanHitLine(other, 1, 1, Player.Center, 1, 1))
                spot = other;
        }

        int windup = Main.rand.Next(ChakraRules.PracticeWindupMinTicks, ChakraRules.PracticeWindupMaxTicks + 1);
        Projectile.NewProjectile(Player.GetSource_Misc("SubstitutionDrill"), spot, Vector2.Zero,
            ModContent.ProjectileType<KakashiPracticeKunai>(), 0, 0f, Player.whoAmI, windup);
        kunaiInFlight = true;
        thrown++;
    }

    private Vector2 PickSpot(int side)
    {
        int distance = Main.rand.Next(ChakraRules.PracticeDistanceMinTiles, ChakraRules.PracticeDistanceMaxTiles + 1);
        int rise = Main.rand.Next(1, 4);
        return Player.Center + new Vector2(side * distance * 16f, -rise * 16f);
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
        kakashi = -1;
        string verdict = ChakraRules.PracticeVerdict(substituted, thrown);
        Main.NewText($"卡卡西：{thrown} 支里替身成功 {substituted} 支。{verdict}", 255, 220, 120);
        CombatText.NewText(mentor.getRect(), Color.White, $"{substituted}/{thrown}");
    }
}
