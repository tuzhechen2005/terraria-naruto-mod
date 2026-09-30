using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;

namespace ShinobiPrototype.Common.Players;

// A character's own steps through the Chūnin Exams (specs/M2_中忍考试篇.spec.md): the exams are a personal
// qualification, so each character sits the written test and takes the scrolls. Decided on the character's own client
// and sent to the server, which needs the stage to spawn candidates and the Rain genin for the right players.
public sealed class ChuninExamPlayer : ModPlayer
{
    public bool Recommended { get; private set; }
    public bool WrittenPassed { get; private set; }
    public bool GaveUpWritten { get; private set; }
    public bool DawnSinceGivingUp { get; private set; }
    public ExamScroll Issued { get; private set; }
    public bool RainAmbushDone { get; private set; }
    public int CandidatesBeaten { get; private set; }
    public bool TowerReached { get; private set; }
    public bool PrelimsPassed { get; private set; }

    private bool wasDay = true;

    public ExamProgress Progress => new(Recommended, WrittenPassed, Issued, TowerReached, PrelimsPassed);

    public ExamStage Stage => ChuninExamRules.Stage(StoryWorld.WaveComplete, KonohaWorld.Site.HasValue, Progress,
        ChuninExamRules.ReadyForFinals(NPC.downedBoss3, Player.statLifeMax), StoryWorld.DownedGaara);

    public bool CanSitWritten => ChuninExamRules.CanSitWritten(GaveUpWritten, DawnSinceGivingUp);

    public override void Initialize()
    {
        Recommended = WrittenPassed = GaveUpWritten = DawnSinceGivingUp = RainAmbushDone = TowerReached = PrelimsPassed = false;
        Issued = ExamScroll.None;
        CandidatesBeaten = 0;
    }

    public void Recommend() => Recommended = true;

    public void PassWritten(int correct)
    {
        if (WrittenPassed)
            return;
        WrittenPassed = true;
        GaveUpWritten = false;
        int copper = ChuninExamRules.WrittenReward(correct);
        if (copper > 0)
            Player.QuickSpawnItem(Player.GetSource_Misc("WrittenExam"), ItemID.SilverCoin, copper / 100);
        Main.NewText($"第一试合格（前九题答对 {correct} 题，得 {copper / 100} 银币）。第二试：死亡森林——" +
                     $"到丛林的第四十四演习场入口领取卷轴。{ExamSiteWorld.GateHint(Player)}", Color.LightGreen);
    }

    public void GiveUpWritten()
    {
        GaveUpWritten = true;
        DawnSinceGivingUp = false;
    }

    // The character took part in beating a candidate. Every third is a squad; a squad may carry the other scroll.
    public void CreditCandidate()
    {
        if (Stage != ExamStage.ForestHunt)
            return;
        CandidatesBeaten++;
        if (CandidatesBeaten % ChuninExamRules.SquadSize != 0)
            return;
        int squads = CandidatesBeaten / ChuninExamRules.SquadSize;
        if (ChuninExamRules.SquadDropsScroll(squads, Main.rand.NextFloat()))
            GiveOtherScroll("考生小队掉落了卷轴");
        else
            Main.NewText($"这一队考生带的卷轴和你的一样。（已击败 {squads} 队）", 180, 200, 170);
    }

    // The last of the Rain genin fell with the character in the fight.
    public void CreditRainTrio()
    {
        if (Stage != ExamStage.ForestHunt || RainAmbushDone)
            return;
        RainAmbushDone = true;
        GiveOtherScroll("雨隐的三人组倒下了，他们带着的正是你缺的那一卷");
    }

    private void GiveOtherScroll(string why)
    {
        ExamScroll other = ChuninExamRules.Other(Issued);
        int type = ScrollType(other);
        if (type <= 0)
            return;
        Player.QuickSpawnItem(Player.GetSource_Misc("ExamScroll"), type);
        Main.NewText($"{why}：得到{(other == ExamScroll.Heaven ? "天之卷" : "地之卷")}！带齐两卷去丛林中部的中央塔。" +
                     ExamSiteWorld.TowerHint(Player), Color.LightGreen);
    }

    // Took part in beating Dosu in the tower.
    public void PassPrelims()
    {
        if (Stage != ExamStage.Prelims)
            return;
        PrelimsPassed = true;
        Main.NewText(Stage == ExamStage.Finals
            ? $"预选赛合格！正式赛：到木叶城墙外的考试会场{ExamSiteWorld.StadiumHint(Player)}。"
            : $"预选赛合格！正式赛前先去变强（击败骷髅王，或生命上限达到 {ChuninExamRules.FinalsLifeThreshold}）。",
            Color.LightGreen);
    }

    public static int ScrollType(ExamScroll scroll) => scroll switch
    {
        ExamScroll.Heaven => ModContent.ItemType<HeavenScroll>(),
        ExamScroll.Earth => ModContent.ItemType<EarthScroll>(),
        _ => 0,
    };

    public override void PostUpdate()
    {
        if (Player.whoAmI != Main.myPlayer)
            return;
        if (GaveUpWritten && !wasDay && Main.dayTime)
            DawnSinceGivingUp = true;
        wasDay = Main.dayTime;
        if (Main.GameUpdateCount % 20 != 0)
            return;

        ExamStage stage = Stage;
        if (stage == ExamStage.ForestGate && ExamSiteWorld.Gate is ExamSite gate &&
            gate.DistanceTiles(Player.Center) <= ChuninExamRules.GateReachTiles)
        {
            Issued = ChuninExamRules.Issue(Main.rand.Next(2));
            Player.QuickSpawnItem(Player.GetSource_Misc("ExamScroll"), ScrollType(Issued));
            string mine = Issued == ExamScroll.Heaven ? "天之卷" : "地之卷";
            string other = Issued == ExamScroll.Heaven ? "地之卷" : "天之卷";
            Main.NewText($"第二试开始！你领到了{mine}。从其他考生手里夺取{other}，带齐两卷到丛林中部的中央塔。" +
                         "不限时——但森林里不只有考生。", new Color(255, 200, 120));
        }
        else if (stage == ExamStage.ForestHunt && ExamSiteWorld.InArena(ExamSiteWorld.Tower, Player.Center) &&
                 ChuninExamRules.HasBoth(Player.CountItem(ModContent.ItemType<HeavenScroll>()),
                     Player.CountItem(ModContent.ItemType<EarthScroll>())))
        {
            TowerReached = true;
            Main.NewText("天地双开——第二试合格！通过的人太多，要在这座塔里先打一场预选赛。", Color.LightGreen);
        }
    }

    // Development shortcut (/m0 exam): put the character at the start of a stage.
    public void SetStageForTesting(ExamStage stage)
    {
        Initialize();
        Recommended = stage >= ExamStage.Written;
        WrittenPassed = stage >= ExamStage.ForestGate;
        Issued = stage >= ExamStage.ForestHunt ? ExamScroll.Heaven : ExamScroll.None;
        RainAmbushDone = stage >= ExamStage.Prelims;
        TowerReached = stage >= ExamStage.Prelims;
        PrelimsPassed = stage >= ExamStage.Training;
    }

    public override void SaveData(TagCompound tag)
    {
        tag["examFlags"] = (byte)Flags;
        if (Issued != ExamScroll.None)
            tag["examScroll"] = (byte)Issued;
        if (CandidatesBeaten > 0)
            tag["examCandidates"] = CandidatesBeaten;
    }

    public override void LoadData(TagCompound tag)
    {
        Flags = tag.GetByte("examFlags");
        Issued = (ExamScroll)tag.GetByte("examScroll");
        CandidatesBeaten = tag.GetInt("examCandidates");
    }

    private BitsByte Flags
    {
        get => new(Recommended, WrittenPassed, GaveUpWritten, DawnSinceGivingUp, RainAmbushDone, TowerReached,
            PrelimsPassed);
        set
        {
            Recommended = value[0];
            WrittenPassed = value[1];
            GaveUpWritten = value[2];
            DawnSinceGivingUp = value[3];
            RainAmbushDone = value[4];
            TowerReached = value[5];
            PrelimsPassed = value[6];
        }
    }

    // Multiplayer: the server keeps a copy for spawning (see DeathForestSystem).
    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)ShinobiPrototype.Packet.ExamSync);
        packet.Write((byte)Player.whoAmI);
        Write(packet);
        packet.Send(toWho, fromWho);
    }

    public override void CopyClientState(ModPlayer targetCopy)
    {
        ChuninExamPlayer copy = (ChuninExamPlayer)targetCopy;
        copy.Flags = Flags;
        copy.Issued = Issued;
        copy.CandidatesBeaten = CandidatesBeaten;
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        ChuninExamPlayer old = (ChuninExamPlayer)clientPlayer;
        if ((byte)old.Flags != (byte)Flags || old.Issued != Issued || old.CandidatesBeaten != CandidatesBeaten)
            SyncPlayer(-1, Main.myPlayer, false);
    }

    private void Write(BinaryWriter writer)
    {
        writer.Write((byte)Flags);
        writer.Write((byte)Issued);
        writer.Write((ushort)System.Math.Min(CandidatesBeaten, ushort.MaxValue));
    }

    internal void Read(BinaryReader reader)
    {
        Flags = reader.ReadByte();
        Issued = (ExamScroll)reader.ReadByte();
        CandidatesBeaten = reader.ReadUInt16();
    }
}
