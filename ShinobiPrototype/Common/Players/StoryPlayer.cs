using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;

namespace ShinobiPrototype.Common.Players;

public sealed class StoryPlayer : ModPlayer
{
    // Whether the "three insignia collected" notice has been shown. Nothing here gates exploration or bosses.
    public bool InsigniaNoticeShown { get; private set; }
    // The village cheers each boss this character helped beat, the next time they come home (user, 2026-09-30: "每次
    // 打完 boss 之后村民都庆祝"). WelcomedHome is the first homecoming from Wave Country, which has its own words.
    public bool WelcomedHome { get; private set; }
    private readonly List<string> pendingBosses = new();
    private int welcomeTicks = -1;
    private readonly List<int> cheering = new();
    private readonly List<string> cheers = new();

    private static readonly string[] Cheers =
    {
        "欢迎回来！", "了不起！", "木叶的骄傲！", "辛苦了！", "一乐拉面，今天我请！", "你回来啦！", "不愧是木叶的忍者！",
    };

    public const string WaveDuoName = "雾隐的鬼人桃地再不斩与白";

    // Development shortcut (/m0 cheer): forget the first Wave homecoming so it can be seen again.
    public void ResetWelcomeForTesting() => WelcomedHome = false;

    // A boss fell with this character in the fight (BossCelebrationNPC, on this character's client).
    public void QueueCelebration(string boss)
    {
        if (!pendingBosses.Contains(boss))
            pendingBosses.Add(boss);
    }

    public override void Initialize()
    {
        InsigniaNoticeShown = false;
        WelcomedHome = false;
    }

    public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
    {
        if (mediumCoreDeath)
            yield break;

        Item kunai = new();
        kunai.SetDefaults(ModContent.ItemType<TrainingKunai>());
        yield return kunai;

        Item handbook = new();
        handbook.SetDefaults(ModContent.ItemType<NinjaHandbook>());
        yield return handbook;

        Item pills = new();
        pills.SetDefaults(ModContent.ItemType<ChakraPill>());
        pills.stack = 3;
        yield return pills;
    }

    public override void OnEnterWorld()
    {
        Main.NewText(CurrentObjective(), 100, 200, 245);
    }

    public override void PostUpdate()
    {
        if (Player.whoAmI != Main.myPlayer)
            return;

        if (pendingBosses.Count > 0 && welcomeTicks < 0 && Main.GameUpdateCount % 30 == 0 && KonohaWorld.InKonoha(Player.Center))
            StartWelcome();
        if (welcomeTicks >= 0)
            Welcome();

        if (!InsigniaNoticeShown && WaveStage == WaveStage.Showdown && Player.CountItem(ModContent.ItemType<MistInsignia>()) >= 3)
        {
            InsigniaNoticeShown = true;
            Main.NewText("已收集三枚雾隐标记。现在可在工作台制作再不斩挑战卷轴，去断桥与再不斩和白决战。", 100, 200, 245);
        }
    }

    // Home after a win: the townsfolk nearby cheer one after another, with confetti over the player.
    private void StartWelcome()
    {
        welcomeTicks = 0;
        cheering.Clear();
        foreach (NPC npc in Main.ActiveNPCs)
            if (npc.townNPC && npc.Distance(Player.Center) < 70 * 16)
                cheering.Add(npc.whoAmI);
        string names = string.Join("、", pendingBosses);
        bool wave = pendingBosses.Contains(WaveDuoName) && !WelcomedHome;
        Main.NewText(wave ? "木叶的大家都听说了——你在波之国，打倒了雾隐的鬼人。" : $"木叶的大家都听说了——你打倒了{names}！",
            new Color(255, 220, 150));
        cheers.Clear();
        cheers.AddRange(Cheers);
        foreach (string boss in pendingBosses)
            cheers.Add(boss == WaveDuoName ? "真的假的，那个桃地再不斩？" : $"连{boss}都被你打倒了？！");
        if (wave)
            WelcomedHome = true;
        pendingBosses.Clear();
        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item4, Player.Center);
    }

    private void Welcome()
    {
        const int spacing = 50;
        if (welcomeTicks < 90 && welcomeTicks % 3 == 0)
            for (int i = 0; i < 3; i++)
                Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-120f, 120f), -90f + Main.rand.NextFloat(-30f, 10f)),
                    DustID.Confetti + Main.rand.Next(4), new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(0.5f, 2f)),
                    0, default, 1.2f);
        if (welcomeTicks % spacing == 0 && welcomeTicks / spacing < cheering.Count)
        {
            NPC npc = Main.npc[cheering[welcomeTicks / spacing]];
            if (npc.active && npc.townNPC)
            {
                Terraria.GameContent.UI.EmoteBubble.NewBubble(Main.rand.Next(new[]
                    {
                        Terraria.GameContent.UI.EmoteID.EmotionLove, Terraria.GameContent.UI.EmoteID.EmoteLaugh,
                        Terraria.GameContent.UI.EmoteID.EmoteHappiness, Terraria.GameContent.UI.EmoteID.PartyBalloons,
                    }), new Terraria.GameContent.UI.WorldUIAnchor(npc), 180);
                int text = CombatText.NewText(npc.getRect(), new Color(255, 230, 160), Main.rand.Next(cheers), true);
                // Long enough to read (user, 2026-09-30: the cheers vanished too fast).
                if (text >= 0 && text < Main.maxCombatText)
                    Main.combatText[text].lifeTime = 300;
            }
        }
        if (++welcomeTicks > spacing * (cheering.Count + 1) + 90)
            welcomeTicks = -1;
    }

    public string CurrentObjective()
    {
        if (StoryWorld.WaveComplete || StoryWorld.DownedGaara)
            return ExamObjective();
        int insignia = System.Math.Min(3, Player.CountItem(ModContent.ItemType<MistInsignia>()));
        string noBridge = WaveBridgeWorld.Site.HasValue ? "" : "（这个世界还没有大桥：先找卡卡西要达兹纳的施工图。）";
        return WaveStage switch
        {
            WaveStage.FindTazuna =>
                $"C 级任务：护送造桥工达兹纳返回波之国。他先回了桥头——海边起雾的地方有座没修完的大桥，去桥头小屋找他。{noBridge}",
            WaveStage.Scout =>
                "C 级任务：远离出生点的地表与海边有雾隐侦察兵出没，击败他们、收集雾隐标记" +
                $"（{insignia}/3）。拿到标记后回大桥一带的海雾里——雾隐的人会找上门来。",
            WaveStage.ReportToTazuna =>
                "伏击你们的是雾隐的中忍，而他们的目标是达兹纳。回桥头小屋，问个清楚。",
            WaveStage.GetStronger =>
                "A 级任务：卡多雇来的是雾隐的鬼人——桃地再不斩。以现在的实力还不够，先去变强" +
                $"（击败克苏鲁之眼，或生命上限达到 {StoryRules.LakeLifeThreshold}）。",
            WaveStage.Lake =>
                $"A 级任务：护送途中，再不斩会在地表的湖边动手。{LakeAmbushSystem.NearestLakeHint(Player)}",
            WaveStage.Bridge =>
                "再不斩被追杀部队带走了——可用千本的追杀部队，不会是来杀他的。雾隐的人一定会去断桥，回大桥看看。",
            _ =>
                $"再不斩还活着，就在断桥的雾里。收集雾隐标记（{insignia}/3），在工作台制作再不斩挑战卷轴（不消耗），" +
                "到大桥一带使用，击败再不斩与白。",
        };
    }

    private string ExamObjective()
    {
        ChuninExamPlayer exam = Player.GetModPlayer<ChuninExamPlayer>();
        switch (exam.Stage)
        {
            case ExamStage.NoVillage:
                return "波之国篇完成。中忍考试在木叶隐村举行，而这个世界没有木叶——中忍考试篇需要新建的世界。其余内容可以继续自由探索。";
            case ExamStage.Recommend:
                return "波之国篇完成。回木叶去——卡卡西有话要说。";
            case ExamStage.Written:
                string lost = Player.HasItem(ModContent.ItemType<ExamAdmissionScroll>()) ? "" : "（推荐书不在身上：找卡卡西再要一份。）";
                return exam.CanSitWritten
                    ? $"中忍考试第一试·笔试：带着推荐书到木叶的忍者学校（阿吽之门往西），找教室里的主考官森乃伊比喜。{lost}"
                    : "第一试：你在第十题放弃了。等明天天亮，再去忍者学校重考。" + lost;
            case ExamStage.ForestGate:
                return $"中忍考试第二试·死亡森林：到丛林边的第四十四演习场入口领取卷轴{ExamSiteWorld.GateHint(Player)}。";
            case ExamStage.ForestHunt:
                string other = ChuninExamRules.Other(exam.Issued) == ExamScroll.Heaven ? "天之卷" : "地之卷";
                if (ChuninExamRules.HasBoth(Player.CountItem(ModContent.ItemType<HeavenScroll>()),
                        Player.CountItem(ModContent.ItemType<EarthScroll>())))
                    return $"第二试：天、地两卷已经齐了，进入丛林中部的中央塔{ExamSiteWorld.TowerHint(Player)}。";
                return $"第二试：从其他考生手里夺取{other}——丛林地表有三人一组的考生小队" +
                       $"（已击败 {exam.CandidatesBeaten / ChuninExamRules.SquadSize} 队）。带齐两卷去中央塔{ExamSiteWorld.TowerHint(Player)}。";
            case ExamStage.Prelims:
                return "第二试合格。预选赛：走进中央塔大厅，与音忍多斯一对一。被他的响鸣穿打中会耳鸣——左右会暂时颠倒。";
            case ExamStage.Training:
                return $"预选赛合格。正式赛在一个月后——先去变强（击败骷髅王，或生命上限达到 {ChuninExamRules.FinalsLifeThreshold}）。";
            case ExamStage.Finals:
                return $"中忍考试正式赛：走进木叶城墙外的考试会场{ExamSiteWorld.StadiumHint(Player)}。对手是砂隐的我爱罗——他的沙会挡下正面的攻击。";
            default:
                return "中忍考试篇完成：我爱罗倒下的那一刻，木叶崩溃开始了。（M3 开发中）";
        }
    }

    // The quest tracker (off by default; ShinobiClientConfig.QuestTracker): a short name for the objective and where
    // it is, when there is one place to go.
    public (string Title, Vector2? Where) Tracker()
    {
        if (StoryWorld.WaveComplete || StoryWorld.DownedGaara)
        {
            ChuninExamPlayer exam = Player.GetModPlayer<ChuninExamPlayer>();
            return exam.Stage switch
            {
                ExamStage.Recommend => ("去找卡卡西", NpcWhere(ModContent.NPCType<Content.NPCs.Kakashi>())),
                ExamStage.Written => ("第一试：忍者学校", KonohaWorld.BuildingWhere("忍者学校")),
                ExamStage.ForestGate => ("第二试：演习场入口", ExamSiteWorld.Where(ExamSiteWorld.Gate)),
                ExamStage.ForestHunt when ChuninExamRules.HasBoth(Player.CountItem(ModContent.ItemType<HeavenScroll>()),
                    Player.CountItem(ModContent.ItemType<EarthScroll>())) => ("第二试：进中央塔", ExamSiteWorld.Where(ExamSiteWorld.Tower)),
                ExamStage.ForestHunt => ("第二试：夺取另一卷（丛林地表）", null),
                ExamStage.Prelims => ("预选赛：中央塔大厅", ExamSiteWorld.Where(ExamSiteWorld.Tower)),
                ExamStage.Training => ("修整：击败骷髅王或生命达到 400", null),
                ExamStage.Finals => ("正式赛：考试会场", ExamSiteWorld.Where(ExamSiteWorld.Stadium)),
                ExamStage.NoVillage => ("中忍考试需要新世界", null),
                _ => ("木叶崩溃（开发中）", null),
            };
        }
        Vector2? bridge = WaveBridgeWorld.Site is BridgeSite site
            ? new Vector2((site.HutMidX != 0 ? site.HutMidX : site.ShoreX) * 16f, site.DeckY * 16f)
            : null;
        return WaveStage switch
        {
            WaveStage.FindTazuna => ("去桥头找达兹纳", bridge),
            WaveStage.Scout => ("击败雾隐侦察兵", null),
            WaveStage.ReportToTazuna => ("回桥头问达兹纳", bridge),
            WaveStage.GetStronger => ("变强：击败克苏鲁之眼或生命达到 200", null),
            WaveStage.Lake => ("去地表的湖边", LakeAmbushSystem.NearestLake(Player)),
            _ => ("去断桥", bridge),
        };
    }

    private static Vector2? NpcWhere(int type)
    {
        foreach (NPC npc in Main.ActiveNPCs)
            if (npc.type == type)
                return npc.Center;
        return null;
    }

    public WaveStage WaveStage => StoryRules.Stage(StoryWorld.WaveComplete, StoryWorld.MetTazuna,
        StoryWorld.DownedDemonBrothers, StoryWorld.TazunaConfessed, StoryWorld.LakeDone,
        Player.GetModPlayer<MistEncounterPlayer>().SawPreview || StoryWorld.ZabuzaFought, ReadyForLake);

    public bool ReadyForLake => StoryRules.ReadyForLake(NPC.downedBoss1, Player.statLifeMax);

    public override void SaveData(TagCompound tag)
    {
        if (InsigniaNoticeShown)
            tag["insigniaNotice"] = true;
        if (WelcomedHome)
            tag["welcomedHome"] = true;
        if (pendingBosses.Count > 0)
            tag["pendingCelebrations"] = new List<string>(pendingBosses);
    }

    public override void LoadData(TagCompound tag)
    {
        // Saves from before the M1 rewrite stored this as journal stage 2.
        InsigniaNoticeShown = tag.GetBool("insigniaNotice") || tag.GetInt("stage") >= 2;
        WelcomedHome = tag.GetBool("welcomedHome");
        pendingBosses.Clear();
        pendingBosses.AddRange(tag.GetList<string>("pendingCelebrations"));
    }
}
