using System.Collections.Generic;
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

    public override void Initialize() => InsigniaNoticeShown = false;

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

        if (!InsigniaNoticeShown && WaveStage == WaveStage.Showdown && Player.CountItem(ModContent.ItemType<MistInsignia>()) >= 3)
        {
            InsigniaNoticeShown = true;
            Main.NewText("已收集三枚雾隐标记。现在可在工作台制作再不斩挑战卷轴，去断桥与再不斩和白决战。", 100, 200, 245);
        }
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
                    ? $"中忍考试第一试·笔试：到木叶的忍者学校（阿吽之门往西），在学校里使用推荐书。{lost}"
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

    public WaveStage WaveStage => StoryRules.Stage(StoryWorld.WaveComplete, StoryWorld.MetTazuna,
        StoryWorld.DownedDemonBrothers, StoryWorld.TazunaConfessed, StoryWorld.LakeDone,
        Player.GetModPlayer<MistEncounterPlayer>().SawPreview || StoryWorld.ZabuzaFought, ReadyForLake);

    public bool ReadyForLake => StoryRules.ReadyForLake(NPC.downedBoss1, Player.statLifeMax);

    public override void SaveData(TagCompound tag)
    {
        if (InsigniaNoticeShown)
            tag["insigniaNotice"] = true;
    }

    public override void LoadData(TagCompound tag)
    {
        // Saves from before the M1 rewrite stored this as journal stage 2.
        InsigniaNoticeShown = tag.GetBool("insigniaNotice") || tag.GetInt("stage") >= 2;
    }
}
