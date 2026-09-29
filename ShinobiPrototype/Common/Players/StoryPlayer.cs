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

        if (!InsigniaNoticeShown && !StoryWorld.WaveComplete && Player.CountItem(ModContent.ItemType<MistInsignia>()) >= 3)
        {
            InsigniaNoticeShown = true;
            Main.NewText(WaveBridgeWorld.Site.HasValue && !Player.GetModPlayer<MistEncounterPlayer>().SawPreview
                ? "已收集三枚雾隐标记。雾隐的人都往海边的大桥去了——去那里看看。挑战卷轴也已经可以在工作台制作了。"
                : "已收集三枚雾隐标记。现在可在工作台制作波之国挑战卷轴，与再不斩和白决战。", 100, 200, 245);
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
        string bridge = WaveBridgeWorld.Site.HasValue
            ? "海边起雾的地方有一座没修完的大桥，造桥工达兹纳就在桥头。"
            : "这个世界还没有大桥：找卡卡西要达兹纳的施工图。";
        return $"C 级任务：护送造桥工返回波之国。{bridge}远离出生点的地表与海边有雾隐侦察兵出没——收集雾隐标记（{insignia}/3），" +
               "在工作台制作再不斩挑战卷轴（不消耗），击败再不斩与白。";
    }

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
