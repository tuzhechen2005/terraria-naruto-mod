using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Items;

namespace ShinobiPrototype.Common.Systems;

// Hosts the Ninja Handbook panel. Later pages (lore, reward and style silhouettes) plug into HandbookState.
public sealed class HandbookSystem : ModSystem
{
    private static UserInterface ui;
    private static HandbookState state;

    public static bool IsOpen => ui?.CurrentState != null;

    public override void Load()
    {
        if (Main.dedServ)
            return;
        ui = new UserInterface();
        state = new HandbookState();
    }

    public override void Unload()
    {
        ui = null;
        state = null;
    }

    public static void Toggle()
    {
        if (IsOpen)
        {
            Close();
            return;
        }

        ui.SetState(state);
        state.Refresh();
    }

    public static void Close()
    {
        if (!IsOpen)
            return;
        ui.SetState(null);
        SoundEngine.PlaySound(SoundID.MenuClose);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        if (!IsOpen)
            return;
        if (Main.gameMenu || Main.LocalPlayer.dead)
            ui.SetState(null);
        else
            ui.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
        layers.Insert(index >= 0 ? index : layers.Count, new LegacyGameInterfaceLayer(
            "ShinobiPrototype: Handbook",
            () =>
            {
                if (IsOpen)
                    ui.Draw(Main.spriteBatch, new GameTime());
                return true;
            },
            InterfaceScaleType.UI));
    }
}

internal sealed class HandbookState : UIState
{
    private enum Page { Mission, Bosses, Jutsu, Chakra, Rewards, Paths, Lore }

    private static readonly (Page Page, string Key)[] Tabs =
    {
        (Page.Mission, "Mission"),
        (Page.Bosses, "Bosses"),
        (Page.Jutsu, "Jutsu"),
        (Page.Chakra, "Chakra"),
        (Page.Rewards, "Rewards"),
        (Page.Paths, "Paths"),
        (Page.Lore, "Lore"),
    };

    // Each path's art, and its text under Handbook.Path.<Key> (Name, Ultimate, Teaser).
    private static readonly (string Icon, string Key)[] Paths =
    {
        ("Style_Sharingan", "Sharingan"),
        ("Style_EightGates", "EightGates"),
        ("Style_Byakugan", "Byakugan"),
        ("Style_Sage", "Sage"),
    };

    private static readonly Color TabIdle = new Color(63, 82, 151) * 0.85f;
    private static readonly Color TabActive = new(200, 150, 60);

    private readonly List<(Page Page, UITextPanel<string> Panel)> tabPanels = new();
    private UIPanel panel;
    private UIText body;
    private HandbookIconGrid icons;
    private Page page = Page.Mission;
    private int refreshTimer;

    public override void OnInitialize()
    {
        panel = new UIPanel();
        panel.Width.Set(720f, 0f);
        panel.Height.Set(420f, 0f);
        panel.HAlign = 0.5f;
        panel.VAlign = 0.5f;
        Append(panel);

        UIText title = new(Loc.Get("Handbook.Title"), 1.1f);
        title.Top.Set(6f, 0f);
        panel.Append(title);

        for (int i = 0; i < Tabs.Length; i++)
        {
            Page target = Tabs[i].Page;
            UITextPanel<string> tab = new(Loc.Get("Handbook.Tab." + Tabs[i].Key), 0.8f);
            tab.Width.Set(76f, 0f);
            tab.Left.Set(104f + i * 80f, 0f);
            tab.OnLeftClick += (_, _) =>
            {
                page = target;
                SoundEngine.PlaySound(SoundID.MenuTick);
                Refresh();
            };
            panel.Append(tab);
            tabPanels.Add((target, tab));
        }

        UITextPanel<string> close = new("×", 0.9f);
        close.Width.Set(36f, 0f);
        close.HAlign = 1f;
        close.OnLeftClick += (_, _) => HandbookSystem.Close();
        panel.Append(close);

        body = new UIText("", 0.9f)
        {
            IsWrapped = true,
            TextOriginX = 0f,
            TextOriginY = 0f,
        };
        body.Top.Set(52f, 0f);
        body.Width.Set(0f, 1f);
        body.Height.Set(-52f, 1f);
        panel.Append(body);

        icons = new HandbookIconGrid();
        icons.Top.Set(56f, 0f);
        icons.Width.Set(0f, 1f);
        icons.Height.Set(120f, 0f);
        panel.Append(icons);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (panel.ContainsPoint(Main.MouseScreen))
            Main.LocalPlayer.mouseInterface = true;
        if (++refreshTimer >= 20)
            Refresh();
    }

    public void Refresh()
    {
        refreshTimer = 0;
        foreach ((Page target, UITextPanel<string> tab) in tabPanels)
            tab.BackgroundColor = target == page ? TabActive : TabIdle;
        icons?.Set(page switch
        {
            Page.Rewards => RewardIcons(),
            Page.Paths => PathIcons(),
            _ => System.Array.Empty<HandbookIconGrid.Entry>(),
        });
        body?.Top.Set(icons == null || icons.IsEmpty ? 52f : 186f, 0f);
        body?.Height.Set(icons == null || icons.IsEmpty ? -52f : -186f, 1f);
        body?.Recalculate();
        FitBody(page switch
        {
            Page.Jutsu => JutsuText(),
            Page.Chakra => ChakraText(),
            Page.Lore => LoreText(),
            Page.Rewards => RewardsText(),
            Page.Paths => PathsText(),
            Page.Bosses => BossesText(),
            _ => MissionText(),
        });
    }

    // The page's text, made smaller until it fits the panel (there is no scrolling, and English runs longer than
    // Chinese).
    private void FitBody(string text)
    {
        if (body == null)
            return;
        DynamicSpriteFont font = Terraria.GameContent.FontAssets.MouseText.Value;
        float width = body.GetInnerDimensions().Width;
        float height = body.GetInnerDimensions().Height;
        float scale = 0.9f;
        while (scale > 0.62f && width > 0f &&
               Terraria.UI.Chat.ChatManager.GetStringSize(font, font.CreateWrappedText(text, width / scale), Vector2.One).Y * scale > height)
            scale -= 0.04f;
        body.SetText(text, scale, false);
    }

    private static string MissionText()
    {
        Player player = Main.LocalPlayer;
        if (StoryWorld.WaveComplete || StoryWorld.DownedGaara)
            return ExamText(player);
        int insignia = System.Math.Min(3, player.CountItem(ModContent.ItemType<MistInsignia>()));
        string bridge = WaveBridgeWorld.Site is null ? Loc.Get("Handbook.Status.NoBridge")
            : WaveBridgeWorld.Finished ? Loc.Get("Handbook.Status.BridgeDone") : Loc.Get("Handbook.Status.BridgeBuilding");
        return player.GetModPlayer<StoryPlayer>().CurrentObjective() + "\n\n" +
               Loc.Get("Handbook.Progress") + "\n" +
               Loc.Get("Handbook.Wave.Insignia", insignia) + "\n" +
               Loc.Get("Handbook.Wave.Duo", Loc.Get(StoryWorld.WaveComplete ? "Handbook.Status.Defeated" : "Handbook.Status.NotDefeated")) + "\n" +
               Loc.Get("Handbook.Wave.Bridge", bridge) + "\n" +
               Loc.Get("Handbook.Wave.Exams", Loc.Get(StoryWorld.WaveComplete ? "Handbook.Status.ExamsOpen" : "Handbook.Status.ExamsLocked")) + "\n\n" +
               Loc.Get("Handbook.AskKakashi");
    }

    // The Chūnin Exams (specs/M2_中忍考试篇.spec.md).
    private static string ExamText(Player player)
    {
        ChuninExamPlayer exam = player.GetModPlayer<ChuninExamPlayer>();
        ExamStage stage = exam.Stage;
        string Mark(ExamStage from) => Loc.Get(stage > from ? "Handbook.Exam.Passed" : stage == from ? "Handbook.Exam.Ongoing" : "Handbook.Exam.NotStarted");
        string forest = stage < ExamStage.ForestGate ? Loc.Get("Handbook.Exam.NotStarted")
            : stage > ExamStage.ForestHunt ? Loc.Get("Handbook.Exam.Passed")
            : exam.Issued == ExamScroll.None ? Loc.Get("Handbook.Exam.ForestFetch")
            : Loc.Get("Handbook.Exam.ForestHunt", Loc.Get(exam.Issued == ExamScroll.Heaven ? "Handbook.Exam.Heaven" : "Handbook.Exam.Earth"),
                exam.SquadsBeaten);
        string finals = Loc.Get(StoryWorld.DownedGaara ? "Handbook.Exam.FinalsOver"
            : stage == ExamStage.Finals ? "Handbook.Exam.FinalsOpen" : "Handbook.Exam.NotStarted");
        string vow = Loc.Get(VowRules.SchoolKey(player.GetModPlayer<StyleCorePlayer>().Vow));
        return Loc.Get("Handbook.Exam.Title") + "\n\n" + player.GetModPlayer<StoryPlayer>().CurrentObjective() + "\n\n" +
               Loc.Get("Handbook.Progress") + "\n" +
               Loc.Get("Handbook.Exam.Recommend", Loc.Get(exam.Recommended ? "Handbook.Exam.Recommended" : "Handbook.Exam.NotRecommended")) + "\n" +
               Loc.Get("Handbook.Exam.Written", Mark(ExamStage.Written)) + "\n" +
               Loc.Get("Handbook.Exam.Forest", forest) + "\n" +
               Loc.Get("Handbook.Exam.Prelims", Mark(ExamStage.Prelims)) + "\n" +
               Loc.Get("Handbook.Exam.Finals", finals) + "\n" +
               Loc.Get("Handbook.Exam.Vow", vow) + "\n\n" +
               Loc.Get("Handbook.AskKakashi");
    }

    // Every Naruto boss: where and how, when, what it drops, and whether it has fallen here (master spec,
    // "可玩性与引导"; the same information goes to the Boss Checklist mod).
    private static string BossesText()
    {
        static string Line(bool downed, string boss) =>
            Loc.Get(downed ? "Handbook.Bosses.Defeated" : "Handbook.Bosses.NotDefeated") + Loc.Get("Handbook.Bosses." + boss);
        return Loc.Get("Handbook.Bosses.Intro") + "\n\n" +
               Line(StoryWorld.DownedDemonBrothers, "Brothers") + "\n" +
               Line(StoryWorld.WaveComplete, "WaveDuo") + "\n" +
               Line(StoryWorld.OrochimaruMet, "Orochimaru") + "\n" +
               Line(StoryWorld.DownedDosu, "Dosu") + "\n" +
               Line(StoryWorld.DownedGaara, "Gaara") + "\n" +
               Line(StoryWorld.DownedNeji, "Neji");
    }

    private static string JutsuText()
    {
        SubstitutionPlayer substitution = Main.LocalPlayer.GetModPlayer<SubstitutionPlayer>();
        float regen = Main.LocalPlayer.GetModPlayer<StyleCorePlayer>().LogRegenTicks / 60f;
        return Loc.Get("Handbook.Jutsu.Title") + "\n" +
               Loc.Get("Handbook.Jutsu.Logs", ChakraRules.StartingLogs) + "\n" +
               Loc.Get("Handbook.Jutsu.Auto", ChakraRules.LogThresholdPercent) + "\n" +
               Loc.Get("Handbook.Jutsu.Regen", regen.ToString("0")) + "\n" +
               Loc.Get("Handbook.Jutsu.Key", ShinobiKeybinds.SubstitutionKeyName(), ChakraRules.SubstitutionLogs,
                   ChakraRules.SubstitutionCost, ChakraRules.StealthTicks / 60) + "\n" +
               Loc.Get("Handbook.Jutsu.Binds") + "\n" +
               Loc.Get("Handbook.Jutsu.Sealed") + "\n" +
               Loc.Get("Handbook.Jutsu.Drill") + "\n\n" +
               Loc.Get("Handbook.Jutsu.Current", substitution.Logs, substitution.MaxLogs);
    }

    private static IEnumerable<HandbookIconGrid.Entry> RewardIcons()
    {
        RewardCollectionPlayer collection = Main.LocalPlayer.GetModPlayer<RewardCollectionPlayer>();
        foreach (int type in RewardCollectionPlayer.WaveRewards)
        {
            Main.instance.LoadItem(type);
            yield return new HandbookIconGrid.Entry(Terraria.GameContent.TextureAssets.Item[type].Value,
                Lang.GetItemNameValue(type), collection.HasSeen(type));
        }
    }

    private static string RewardsText()
    {
        RewardCollectionPlayer collection = Main.LocalPlayer.GetModPlayer<RewardCollectionPlayer>();
        int seen = 0;
        foreach (int type in RewardCollectionPlayer.WaveRewards)
            if (collection.HasSeen(type))
                seen++;
        return Loc.Get("Handbook.Rewards.Title", seen, RewardCollectionPlayer.WaveRewards.Length) + "\n\n" +
               Loc.Get("Handbook.Rewards.Weapon") + "\n" +
               Loc.Get("Handbook.Rewards.Masks") + "\n" +
               Loc.Get("Handbook.Rewards.FirstWin") + "\n" +
               Loc.Get("Handbook.Rewards.Expert");
    }

    private static IEnumerable<HandbookIconGrid.Entry> PathIcons()
    {
        foreach ((string icon, string key) in Paths)
            yield return new HandbookIconGrid.Entry(
                ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>($"ShinobiPrototype/Assets/Handbook/{icon}",
                    ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, Loc.Get($"Handbook.Path.{key}.Name"), Unlocked: false, Tiers: 3);
    }

    private static string PathsText()
    {
        string lines = "";
        foreach ((_, string key) in Paths)
            lines += Loc.Get("Handbook.Path.Line", Loc.Get($"Handbook.Path.{key}.Name"), Loc.Get($"Handbook.Path.{key}.Teaser"),
                Loc.Get($"Handbook.Path.{key}.Ultimate")) + "\n";
        return Loc.Get("Handbook.Path.Intro") + "\n\n" + lines + "\n" + Loc.Get("Handbook.Path.Outro");
    }

    // Records unlocked by beating each story boss; the story is told here rather than in cutscenes.
    private static string LoreText() => Loc.Get(StoryWorld.WaveComplete ? "Handbook.Lore.Wave" : "Handbook.Lore.Empty");

    private static string ChakraText()
    {
        ChakraPlayer chakra = Main.LocalPlayer.GetModPlayer<ChakraPlayer>();
        return Loc.Get("Handbook.Chakra.Header", chakra.Chakra, chakra.MaxChakra, chakra.Crystals, ChakraRules.MaxCrystals) + "\n\n" +
               Loc.Get("Handbook.Chakra.Recovery") + "\n" +
               Loc.Get("Handbook.Chakra.Natural", ChakraRules.SafeRegenPerSecond.ToString("0"), ChakraRules.RegenDelayTicks / 60,
                   ChakraRules.CombatRegenPerSecond.ToString("0")) + "\n" +
               Loc.Get("Handbook.Chakra.OnHit", ChakraRules.HitRegenPerHit, ChakraRules.HitRegenPerSecondCap) + "\n" +
               Loc.Get("Handbook.Chakra.Pill", ChakraRules.PillRestore, ChakraRules.PillSicknessTicks / 60) + "\n\n" +
               Loc.Get("Handbook.Chakra.Raise") + "\n" +
               Loc.Get("Handbook.Chakra.Crystal", ChakraRules.CrystalBonus, ChakraRules.PreHardmodeMaxChakra);
    }
}
