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
    // The squad lying in wait past the gate has been beaten (the second test's first encounter).
    public bool GateSquadDone { get; private set; }
    // Orochimaru has been met halfway through the forest (and left); Kakashi and Anko have had their word about it.
    public bool OrochimaruDone { get; private set; }
    public bool KakashiHeardOrochimaru { get; set; }
    public bool AnkoHeardOrochimaru { get; set; }
    public int SquadsBeaten { get; private set; }
    public bool TowerReached { get; private set; }
    public bool PrelimsPassed { get; private set; }
    // The Eight Gates core and Lee's leg weights come with each character's first win over Gaara.
    public bool GaaraFirstWin { get; private set; }

    public void ClaimGaaraFirstWin() => GaaraFirstWin = true;

    private bool wasDay = true;
    private ExamStage? lastStage;

    public ExamProgress Progress => new(Recommended, WrittenPassed, Issued, TowerReached, PrelimsPassed);

    public ExamStage Stage => ChuninExamRules.Stage(StoryWorld.WaveComplete, KonohaWorld.Site.HasValue, Progress,
        ChuninExamRules.ReadyForFinals(NPC.downedBoss3, Player.statLifeMax), StoryWorld.DownedGaara);

    public bool CanSitWritten => ChuninExamRules.CanSitWritten(GaveUpWritten, DawnSinceGivingUp);

    public override void Initialize()
    {
        Recommended = WrittenPassed = GaveUpWritten = DawnSinceGivingUp = RainAmbushDone = TowerReached = PrelimsPassed =
            GateSquadDone = OrochimaruDone = KakashiHeardOrochimaru = AnkoHeardOrochimaru = false;
        Issued = ExamScroll.None;
        SquadsBeaten = 0;
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
        Main.NewText(Loc.Get("Exam.WrittenPassed", correct, copper / 100, ExamSiteWorld.GateHint(Player)), Color.LightGreen);
    }

    public void GiveUpWritten()
    {
        GaveUpWritten = true;
        DawnSinceGivingUp = false;
    }

    public bool HasBothScrolls => ChuninExamRules.HasBoth(Player.CountItem(ModContent.ItemType<HeavenScroll>()),
        Player.CountItem(ModContent.ItemType<EarthScroll>()));

    // The character took part in beating the squad that lay in wait past the gate: they carried the same scroll, and
    // one of them lets slip where the other kind is to be had (not spelled out, user 2026-10-01).
    public void CreditGateSquad()
    {
        if (Stage != ExamStage.ForestHunt || GateSquadDone)
            return;
        GateSquadDone = true;
        SquadsBeaten++;
        Main.NewText(Loc.Get("Exam.GateSquad", ScrollName(Issued), ScrollName(ChuninExamRules.Other(Issued))), 180, 200, 170);
    }

    // Took part in the encounter with Orochimaru (he left: held off, held out against, or the player fell). The first
    // time he leaves the Sharingan in a vial behind (user, 2026-10-01: like the Eight Gates from Gaara).
    public void CreditOrochimaru()
    {
        if (Stage != ExamStage.ForestHunt || OrochimaruDone)
            return;
        OrochimaruDone = true;
        vialPending = true;
    }

    // Handed over once the player is up again (they may have fallen in the fight).
    private bool vialPending;

    private void HandOverVial()
    {
        if (!vialPending || Player.dead)
            return;
        vialPending = false;
        Player.QuickSpawnItem(Player.GetSource_Misc("Orochimaru"), ModContent.ItemType<Content.Items.StyleCores.SharinganCore1>());
        Main.NewText(Loc.Get("Exam.Vial"), new Color(190, 150, 230));
    }

    // A squad met anywhere else in the forest: an optional fight, no scroll, no announcement.
    public void CreditSquad()
    {
        if (Stage == ExamStage.ForestHunt)
            SquadsBeaten++;
    }

    public static string ScrollName(ExamScroll scroll) => Loc.Get(scroll == ExamScroll.Heaven ? "Handbook.Exam.Heaven" : "Handbook.Exam.Earth");

    // The last of the Rain genin fell with the character in the fight.
    public void CreditRainTrio()
    {
        if (Stage != ExamStage.ForestHunt || RainAmbushDone)
            return;
        RainAmbushDone = true;
        GiveOtherScroll(Loc.Get("Exam.RainTrioDown"));
    }

    private void GiveOtherScroll(string why)
    {
        ExamScroll other = ChuninExamRules.Other(Issued);
        int type = ScrollType(other);
        if (type <= 0 || HasBothScrolls)
            return;
        Player.QuickSpawnItem(Player.GetSource_Misc("ExamScroll"), type);
        Main.NewText(Loc.Get("Exam.GotScroll", why, ScrollName(other), ExamSiteWorld.TowerHint(Player)), Color.LightGreen);
    }

    // Mitarashi Anko hands over one of the two scrolls at the gate (specs/M2_中忍考试篇.spec.md 3.2).
    public void IssueScroll()
    {
        if (Stage != ExamStage.ForestGate)
            return;
        Issued = ChuninExamRules.Issue(Main.rand.Next(2));
        Player.QuickSpawnItem(Player.GetSource_Misc("ExamScroll"), ScrollType(Issued));
    }

    // Took part in beating Dosu in the tower.
    public void PassPrelims()
    {
        if (Stage != ExamStage.Prelims)
            return;
        PrelimsPassed = true;
        Main.NewText(Stage == ExamStage.Finals
            ? Loc.Get("Exam.PrelimsPassedFinals", ExamSiteWorld.StadiumHint(Player))
            : Loc.Get("Exam.PrelimsPassedTraining", ChuninExamRules.FinalsLifeThreshold),
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
        HandOverVial();
        if (GaveUpWritten && !wasDay && Main.dayTime)
            DawnSinceGivingUp = true;
        wasDay = Main.dayTime;
        if (Main.GameUpdateCount % 20 != 0)
            return;

        ExamStage stage = Stage;
        // A line when the finals open, as quiet as vanilla's progress messages.
        if (lastStage == ExamStage.Training && stage == ExamStage.Finals)
            Main.NewText(Loc.Get("Exam.FinalsPosted"), new Color(255, 220, 150));
        lastStage = stage;
        if (stage == ExamStage.ForestHunt && ExamSiteWorld.InArena(ExamSiteWorld.Tower, Player.Center) &&
                 ChuninExamRules.HasBoth(Player.CountItem(ModContent.ItemType<HeavenScroll>()),
                     Player.CountItem(ModContent.ItemType<EarthScroll>())))
        {
            TowerReached = true;
            Main.NewText(Loc.Get("Exam.TowerReached"), Color.LightGreen);
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
        GateSquadDone = stage >= ExamStage.Prelims;
        OrochimaruDone = stage >= ExamStage.Prelims;
        TowerReached = stage >= ExamStage.Prelims;
        PrelimsPassed = stage >= ExamStage.Training;
    }

    public override void SaveData(TagCompound tag)
    {
        tag["examFlags"] = (byte)Flags;
        if (Issued != ExamScroll.None)
            tag["examScroll"] = (byte)Issued;
        if (SquadsBeaten > 0)
            tag["examSquads"] = SquadsBeaten;
        tag["examFlags2"] = (byte)Flags2;
        if (GaaraFirstWin)
            tag["gaaraFirstWin"] = true;
    }

    public override void LoadData(TagCompound tag)
    {
        Flags = tag.GetByte("examFlags");
        Issued = (ExamScroll)tag.GetByte("examScroll");
        SquadsBeaten = tag.GetInt("examSquads");
        Flags2 = tag.GetByte("examFlags2");
        GaaraFirstWin = tag.GetBool("gaaraFirstWin");
    }

    private BitsByte Flags
    {
        get => new(Recommended, WrittenPassed, GaveUpWritten, DawnSinceGivingUp, RainAmbushDone, TowerReached,
            PrelimsPassed, GateSquadDone);
        set
        {
            Recommended = value[0];
            WrittenPassed = value[1];
            GaveUpWritten = value[2];
            DawnSinceGivingUp = value[3];
            RainAmbushDone = value[4];
            TowerReached = value[5];
            PrelimsPassed = value[6];
            GateSquadDone = value[7];
        }
    }

    private BitsByte Flags2
    {
        get => new(OrochimaruDone, KakashiHeardOrochimaru, AnkoHeardOrochimaru);
        set
        {
            OrochimaruDone = value[0];
            KakashiHeardOrochimaru = value[1];
            AnkoHeardOrochimaru = value[2];
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
        copy.Flags2 = Flags2;
        copy.Issued = Issued;
        copy.SquadsBeaten = SquadsBeaten;
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        ChuninExamPlayer old = (ChuninExamPlayer)clientPlayer;
        if ((byte)old.Flags != (byte)Flags || (byte)old.Flags2 != (byte)Flags2 || old.Issued != Issued || old.SquadsBeaten != SquadsBeaten)
            SyncPlayer(-1, Main.myPlayer, false);
    }

    private void Write(BinaryWriter writer)
    {
        writer.Write((byte)Flags);
        writer.Write((byte)Flags2);
        writer.Write((byte)Issued);
        writer.Write((ushort)System.Math.Min(SquadsBeaten, ushort.MaxValue));
    }

    internal void Read(BinaryReader reader)
    {
        Flags = reader.ReadByte();
        Flags2 = reader.ReadByte();
        Issued = (ExamScroll)reader.ReadByte();
        SquadsBeaten = reader.ReadUInt16();
    }
}
