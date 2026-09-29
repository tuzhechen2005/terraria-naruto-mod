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
    // Legacy journal milestones; none of these gates exploration or boss summoning.
    public int Stage { get; private set; }
    public int KunaiHits { get; private set; }
    public int CombatStyle { get; private set; }
    public int ChakraNature { get; private set; }
    public int LearnedNatures { get; private set; }
    public int ManualNature { get; private set; }
    public int ActiveNature { get; private set; }
    public int ExamStage { get; private set; }

    public override void Initialize()
    {
        Stage = 0;
        KunaiHits = 0;
        CombatStyle = 0;
        ChakraNature = 0;
        LearnedNatures = 0;
        ManualNature = 1;
        ActiveNature = 0;
        ExamStage = ExamRules.NotRegistered;
    }

    public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
    {
        if (mediumCoreDeath)
            yield break;

        Item kunai = new();
        kunai.SetDefaults(ModContent.ItemType<TrainingKunai>());
        yield return kunai;

        Item mission = new();
        mission.SetDefaults(ModContent.ItemType<MissionScroll>());
        yield return mission;

        Item profile = new();
        profile.SetDefaults(ModContent.ItemType<NinjaProfileScroll>());
        yield return profile;
    }

    public override void OnEnterWorld()
    {
        Main.NewText(CurrentObjective(), 100, 200, 245);
    }

    public override void PostUpdate()
    {
        if (Player.whoAmI != Main.myPlayer)
            return;

        if (Stage == 1 && Player.CountItem(ModContent.ItemType<MistInsignia>()) >= 3)
        {
            Stage = 2;
            Main.NewText("已收集三枚雾隐标记。现在可在工作台制作波之国挑战卷轴，与再不斩和白决战。", 100, 200, 245);
        }

        if (ExamStage == ExamRules.ForestTrial &&
            ExamRules.HasBothScrolls(Player.CountItem(ModContent.ItemType<HeavenScroll>()),
                Player.CountItem(ModContent.ItemType<EarthScroll>())))
        {
            ExamStage = ExamRules.ArenaReady;
            Main.NewText("已集齐天、地卷轴！在工作台制作预选赛挑战书。", 100, 220, 160);
        }
    }

    public void RegisterKunaiHit(NPC target)
    {
        if (Player.whoAmI != Main.myPlayer || Stage != 0 || target.friendly || target.lifeMax <= 5)
            return;

        KunaiHits = System.Math.Min(3, KunaiHits + 1);
        if (KunaiHits < 3)
        {
            Main.NewText($"基础训练：苦无命中 {KunaiHits}/3", 100, 200, 245);
            return;
        }

        TryGraduate();
        if (Stage == 0)
            Main.NewText("苦无训练已达标。请用身份卷轴选择战斗倾向和查克拉性质。", 100, 200, 245);
    }

    public void CycleStyle()
    {
        if (Stage != 0)
            return;
        CombatStyle = TrainingRules.Next(CombatStyle, TrainingRules.StyleCount);
    }

    public void CycleNature()
    {
        if (Stage != 0)
            return;
        ChakraNature = TrainingRules.Next(ChakraNature, TrainingRules.NatureCount);
    }

    private void TryGraduate()
    {
        if (Stage != 0 || !TrainingRules.CanGraduate(KunaiHits, CombatStyle, ChakraNature))
            return;

        Stage = 1;
        Player.QuickSpawnItem(Player.GetSource_Misc("ShinobiTraining"), ModContent.ItemType<ChakraPalm>());
        Main.NewText("基础训练完成，获得查克拉冲击。探索雾隐线索、挑战首领或继续原版冒险，顺序由你决定。", 100, 200, 245);
    }

    public bool HasLearned(int nature) => nature is >= 1 and <= 5 && (LearnedNatures & (1 << nature)) != 0;

    public void Learn(int nature)
    {
        LearnedNatures |= 1 << nature;
        ActiveNature = nature;
    }

    public void CycleManualNature() => ManualNature = TrainingRules.Next(ManualNature, TrainingRules.NatureCount);

    public void CycleActiveNature()
    {
        for (int i = 0; i < TrainingRules.NatureCount; i++)
        {
            int next = TrainingRules.Next(ActiveNature, TrainingRules.NatureCount);
            ActiveNature = next;
            if (HasLearned(next))
                return;
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
        string training = Stage == 0
            ? $"可选训练：身份卷轴选择倾向/性质（{TrainingRules.StyleName(CombatStyle)}/{TrainingRules.NatureName(ChakraNature)}），苦无命中 {KunaiHits}/3。"
            : "训练已完成。";
        return $"波之国主线：挑战再不斩，鬼人形态下白将入场协战；同一场战斗击败两人才能进入中忍考试。远离出生点的地表有雾隐侦察兵；挑战卷轴在工作台制作且不消耗。{training}";
    }

    public override void SaveData(TagCompound tag)
    {
        if (Stage > 0)
            tag["stage"] = Stage;
        if (KunaiHits > 0)
            tag["kunaiHits"] = KunaiHits;
        if (CombatStyle > 0)
            tag["combatStyle"] = CombatStyle;
        if (ChakraNature > 0)
            tag["chakraNature"] = ChakraNature;
        if (LearnedNatures > 0)
            tag["learnedNatures"] = LearnedNatures;
        if (ManualNature > 1)
            tag["manualNature"] = ManualNature;
        if (ActiveNature > 0)
            tag["activeNature"] = ActiveNature;
        if (ExamStage > 0)
            tag["examStage"] = ExamStage;
    }

    public override void LoadData(TagCompound tag)
    {
        Stage = tag.GetInt("stage");
        KunaiHits = tag.GetInt("kunaiHits");
        CombatStyle = tag.GetInt("combatStyle");
        ChakraNature = tag.GetInt("chakraNature");
        LearnedNatures = tag.GetInt("learnedNatures");
        ManualNature = tag.GetInt("manualNature");
        if (ManualNature is < 1 or > 5)
            ManualNature = 1;
        ActiveNature = tag.GetInt("activeNature");
        ExamStage = tag.GetInt("examStage");
    }
}
