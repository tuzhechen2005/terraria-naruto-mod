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

    private const int CheerCount = 7;   // Story.Cheer1..7

    // The Wave pair in the celebration queue (an id, shown as Story.WaveDuoName). Saves made before the text moved to
    // the localization files hold the old Chinese name, read back as this id.
    public const string WaveDuoName = "WaveDuo";
    private const string LegacyWaveDuoName = "雾隐的鬼人桃地再不斩与白";   // text-check: allow (old saves)

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
            Main.NewText(Loc.Get("Story.InsigniaReady"), 100, 200, 245);
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
        static string Shown(string boss) => boss == WaveDuoName ? Loc.Get("Story.WaveDuoName") : boss;
        string names = string.Join(Loc.Get("Story.NameSeparator"), pendingBosses.ConvertAll(Shown));
        bool wave = pendingBosses.Contains(WaveDuoName) && !WelcomedHome;
        Main.NewText(wave ? Loc.Get("Story.WelcomeWave") : Loc.Get("Story.WelcomeBosses", names), new Color(255, 220, 150));
        cheers.Clear();
        for (int i = 1; i <= CheerCount; i++)
            cheers.Add(Loc.Get($"Story.Cheer{i}"));
        foreach (string boss in pendingBosses)
            cheers.Add(boss == WaveDuoName ? Loc.Get("Story.CheerWave") : Loc.Get("Story.CheerBoss", boss));
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
        string noBridge = WaveBridgeWorld.Site.HasValue ? "" : Loc.Get("Story.Objective.NoBridge");
        return WaveStage switch
        {
            WaveStage.FindTazuna => Loc.Get("Story.Objective.FindTazuna", noBridge),
            WaveStage.Scout => Loc.Get("Story.Objective.Scout"),
            WaveStage.ReportToTazuna => Loc.Get("Story.Objective.ReportToTazuna"),
            WaveStage.GetStronger => Loc.Get("Story.Objective.GetStronger", StoryRules.LakeLifeThreshold),
            WaveStage.Lake => Loc.Get("Story.Objective.Lake", LakeAmbushSystem.NearestLakeHint(Player)),
            WaveStage.Bridge => Loc.Get("Story.Objective.Bridge"),
            _ => Loc.Get("Story.Objective.Showdown", insignia),
        };
    }

    private string ExamObjective()
    {
        ChuninExamPlayer exam = Player.GetModPlayer<ChuninExamPlayer>();
        switch (exam.Stage)
        {
            case ExamStage.NoVillage:
                return Loc.Get("Story.Objective.NoVillage");
            case ExamStage.Recommend:
                return Loc.Get("Story.Objective.Recommend");
            case ExamStage.Written:
                string lost = Player.HasItem(ModContent.ItemType<ExamAdmissionScroll>()) ? "" : Loc.Get("Story.Objective.LostRecommendation");
                return Loc.Get(exam.CanSitWritten ? "Story.Objective.Written" : "Story.Objective.WrittenRetry", lost);
            case ExamStage.ForestGate:
                return Loc.Get("Story.Objective.ForestGate", ExamSiteWorld.GateHint(Player));
            case ExamStage.ForestHunt:
                string other = Loc.Get(ChuninExamRules.Other(exam.Issued) == ExamScroll.Heaven ? "Handbook.Exam.Heaven" : "Handbook.Exam.Earth");
                if (ChuninExamRules.HasBoth(Player.CountItem(ModContent.ItemType<HeavenScroll>()),
                        Player.CountItem(ModContent.ItemType<EarthScroll>())))
                    return Loc.Get("Story.Objective.ForestBoth", ExamSiteWorld.TowerHint(Player));
                return Loc.Get("Story.Objective.ForestHunt", other, exam.SquadsBeaten, ExamSiteWorld.TowerHint(Player));
            case ExamStage.Prelims:
                return Loc.Get("Story.Objective.Prelims");
            case ExamStage.Training:
                return Loc.Get("Story.Objective.Training", ChuninExamRules.FinalsLifeThreshold);
            case ExamStage.Finals:
                return Loc.Get("Story.Objective.Finals", ExamSiteWorld.StadiumHint(Player));
            default:
                return Loc.Get("Story.Objective.Done");
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
                ExamStage.Recommend => (Loc.Get("Story.Tracker.Kakashi"), NpcWhere(ModContent.NPCType<Content.NPCs.Kakashi>())),
                ExamStage.Written => (Loc.Get("Story.Tracker.Written"), KonohaWorld.BuildingWhere(KonohaBuildings.Academy)),
                ExamStage.ForestGate => (Loc.Get("Story.Tracker.ForestGate"), ExamSiteWorld.Where(ExamSiteWorld.Gate)),
                ExamStage.ForestHunt when ChuninExamRules.HasBoth(Player.CountItem(ModContent.ItemType<HeavenScroll>()),
                    Player.CountItem(ModContent.ItemType<EarthScroll>())) => (Loc.Get("Story.Tracker.Tower"), ExamSiteWorld.Where(ExamSiteWorld.Tower)),
                ExamStage.ForestHunt => (Loc.Get("Story.Tracker.Hunt"), null),
                ExamStage.Prelims => (Loc.Get("Story.Tracker.Prelims"), ExamSiteWorld.Where(ExamSiteWorld.Tower)),
                ExamStage.Training => (Loc.Get("Story.Tracker.Training", ChuninExamRules.FinalsLifeThreshold), null),
                ExamStage.Finals => (Loc.Get("Story.Tracker.Finals"), ExamSiteWorld.Where(ExamSiteWorld.Stadium)),
                ExamStage.NoVillage => (Loc.Get("Story.Tracker.NoVillage"), null),
                _ => (Loc.Get("Story.Tracker.Destruction"), null),
            };
        }
        Vector2? bridge = WaveBridgeWorld.Site is BridgeSite site
            ? new Vector2((site.HutMidX != 0 ? site.HutMidX : site.ShoreX) * 16f, site.DeckY * 16f)
            : null;
        return WaveStage switch
        {
            WaveStage.FindTazuna => (Loc.Get("Story.Tracker.FindTazuna"), bridge),
            WaveStage.Scout => (Loc.Get("Story.Tracker.Scout"), bridge),
            WaveStage.ReportToTazuna => (Loc.Get("Story.Tracker.ReportToTazuna"), bridge),
            WaveStage.GetStronger => (Loc.Get("Story.Tracker.GetStronger", StoryRules.LakeLifeThreshold), null),
            WaveStage.Lake => (Loc.Get("Story.Tracker.Lake"), LakeAmbushSystem.NearestLake(Player)),
            _ => (Loc.Get("Story.Tracker.Bridge"), bridge),
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
        foreach (string boss in tag.GetList<string>("pendingCelebrations"))
            pendingBosses.Add(boss == LegacyWaveDuoName ? WaveDuoName : boss);
    }
}
