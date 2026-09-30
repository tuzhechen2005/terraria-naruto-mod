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
    public int ExamStage { get; private set; }

    public override void Initialize()
    {
        InsigniaNoticeShown = false;
        ExamStage = ExamRules.NotRegistered;
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

        if (!InsigniaNoticeShown && WaveStage == WaveStage.Showdown && Player.CountItem(ModContent.ItemType<MistInsignia>()) >= 3)
        {
            InsigniaNoticeShown = true;
            Main.NewText("已收集三枚雾隐标记。现在可在工作台制作再不斩挑战卷轴，去断桥与再不斩和白决战。", 100, 200, 245);
        }

        if (ExamStage == ExamRules.ForestTrial &&
            ExamRules.HasBothScrolls(Player.CountItem(ModContent.ItemType<HeavenScroll>()),
                Player.CountItem(ModContent.ItemType<EarthScroll>())))
        {
            ExamStage = ExamRules.ArenaReady;
            Main.NewText("已集齐天、地卷轴！在工作台制作预选赛挑战书。", 100, 220, 160);
        }
    }

    public bool RegisterExam()
    {
        if (!ExamRules.CanRegister(StoryWorld.WaveComplete, ExamStage))
            return false;
        ExamStage = ExamRules.ForestTrial;
        return true;
    }

    public string CurrentObjective()
    {
        if (StoryWorld.DownedArenaRival)
            return "中忍考试篇完成：你已晋升中忍。下一篇章仍在开发中。";
        if (StoryWorld.WaveComplete)
        {
            return ExamStage switch
            {
                ExamRules.NotRegistered => "波之国双首领战已完成。若想参加中忍考试，可用 5 木材与 1 铁锭或铅锭制作报名书；也可继续自由探索。",
                ExamRules.ForestTrial => "中忍考试：丛林地表击败林地考生取得天之卷轴；地下丛林击败潜伏考生取得地之卷轴。",
                _ => "中忍考试：在工作台用天、地卷轴、10 木材与 3 铁锭或铅锭制作预选赛挑战书；遗失可重新挑战丛林考生。"
            };
        }
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

    public WaveStage WaveStage => StoryRules.Stage(StoryWorld.WaveComplete, StoryWorld.MetTazuna,
        StoryWorld.DownedDemonBrothers, StoryWorld.TazunaConfessed, StoryWorld.LakeDone,
        Player.GetModPlayer<MistEncounterPlayer>().SawPreview, ReadyForLake);

    public bool ReadyForLake => StoryRules.ReadyForLake(NPC.downedBoss1, Player.statLifeMax);

    public override void SaveData(TagCompound tag)
    {
        if (InsigniaNoticeShown)
            tag["insigniaNotice"] = true;
        if (ExamStage > 0)
            tag["examStage"] = ExamStage;
    }

    public override void LoadData(TagCompound tag)
    {
        // Saves from before the M1 rewrite stored this as journal stage 2.
        InsigniaNoticeShown = tag.GetBool("insigniaNotice") || tag.GetInt("stage") >= 2;
        ExamStage = tag.GetInt("examStage");
    }
}
