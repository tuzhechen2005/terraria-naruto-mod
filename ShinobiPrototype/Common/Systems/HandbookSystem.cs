using System.Collections.Generic;
using Microsoft.Xna.Framework;
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

    private static readonly (Page Page, string Name)[] Tabs =
    {
        (Page.Mission, "任务"),
        (Page.Bosses, "首领"),
        (Page.Jutsu, "忍术"),
        (Page.Chakra, "查克拉"),
        (Page.Rewards, "本章奖励"),
        (Page.Paths, "忍道"),
        (Page.Lore, "卷宗"),
    };

    private static readonly (string Icon, string Path, string Ultimate, string Teaser)[] Paths =
    {
        ("Style_Sharingan", "写轮眼", "轮回眼 · 完全体须佐能乎", "看穿一切的眼睛。"),
        ("Style_EightGates", "八门", "死门 · 夜凯", "燃尽生命的青春。"),
        ("Style_Byakugan", "白眼 · 柔拳", "转生眼", "看透经络的眼睛。"),
        ("Style_Sage", "仙术 · 九尾", "尾兽模式", "与自然和尾兽共鸣。"),
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

        UIText title = new("忍者手册", 1.1f);
        title.Top.Set(6f, 0f);
        panel.Append(title);

        for (int i = 0; i < Tabs.Length; i++)
        {
            Page target = Tabs[i].Page;
            UITextPanel<string> tab = new(Tabs[i].Name, 0.8f);
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
        body?.SetText(page switch
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

    private static string MissionText()
    {
        Player player = Main.LocalPlayer;
        if (StoryWorld.WaveComplete || StoryWorld.DownedGaara)
            return ExamText(player);
        int insignia = System.Math.Min(3, player.CountItem(ModContent.ItemType<MistInsignia>()));
        return player.GetModPlayer<StoryPlayer>().CurrentObjective() + "\n\n" +
               "进度\n" +
               $"· 雾隐标记：{insignia}/3（鬼之兄弟各掉一枚，达兹纳坦白时给一枚；之后下雨时在海雾里还能遇到鬼之兄弟）\n" +
               $"· 再不斩与白：{(StoryWorld.WaveComplete ? "已击败" : "未击败")}\n" +
               $"· 海边大桥：{(WaveBridgeWorld.Site is null ? "这个世界还没有（找卡卡西要施工图）" : WaveBridgeWorld.Finished ? "已完工" : "未完工，桥头有造桥工达兹纳")}\n" +
               $"· 中忍考试：{(StoryWorld.WaveComplete ? "已开放" : "击败再不斩与白后开放")}\n\n" +
               "不知道下一步做什么，可以去问卡卡西。";
    }

    // The Chūnin Exams (specs/M2_中忍考试篇.spec.md).
    private static string ExamText(Player player)
    {
        ChuninExamPlayer exam = player.GetModPlayer<ChuninExamPlayer>();
        ExamStage stage = exam.Stage;
        string Mark(ExamStage from) => stage > from ? "合格" : stage == from ? "进行中" : "未开始";
        string forest = stage < ExamStage.ForestGate ? "未开始"
            : stage > ExamStage.ForestHunt ? "合格"
            : exam.Issued == ExamScroll.None ? "进行中（去入口领卷）"
            : $"进行中（持有{(exam.Issued == ExamScroll.Heaven ? "天之卷" : "地之卷")}，已击败考生 {exam.SquadsBeaten} 队）";
        string vow = VowRules.SchoolName(player.GetModPlayer<StyleCorePlayer>().Vow);
        return "中忍考试\n\n" + player.GetModPlayer<StoryPlayer>().CurrentObjective() + "\n\n" +
               "进度\n" +
               $"· 推荐：{(exam.Recommended ? "卡卡西已推荐" : "未推荐")}\n" +
               $"· 第一试·笔试：{Mark(ExamStage.Written)}\n" +
               $"· 第二试·死亡森林：{forest}\n" +
               $"· 预选赛：{Mark(ExamStage.Prelims)}\n" +
               $"· 正式赛：{(StoryWorld.DownedGaara ? "已结束" : stage == ExamStage.Finals ? "可以参加" : "未开始")}\n" +
               $"· 立志：{vow}（取得流派核心后，找火影楼里的三代火影）\n\n" +
               "不知道下一步做什么，可以去问卡卡西。";
    }

    // Every Naruto boss: where and how, when, what it drops, and whether it has fallen here (master spec,
    // "可玩性与引导"; the same information goes to the Boss Checklist mod).
    private static string BossesText()
    {
        static string Done(bool downed) => downed ? "【已击败】" : "【未击败】";
        return "首领（每个首领都能用召唤物重复挑战；剧情只负责第一次）\n\n" +
               $"{Done(StoryWorld.DownedDemonBrothers)}鬼之兄弟 —— 带着雾隐标记进大桥一带的海雾，第一次必定伏击；之后下雨时可能再遇。掉雾隐标记。\n" +
               $"{Done(StoryWorld.WaveComplete)}再不斩与白 —— 克苏鲁之眼后。雾隐标记 ×3 + 木材合成再不斩挑战卷轴，在大桥一带使用。掉职业武器、面具、查克拉结晶。\n" +
               $"{Done(StoryWorld.OrochimaruMet)}大蛇丸（遭遇战）—— 世吞或克脑后（或生命 ≥ 300）。中忍考试第二试中在丛林地表遇到；之后用蛇的蜕皮（藤蔓、丛林孢子、毒刺，铁砧）在丛林地表召唤。打到一半他会离开。掉写轮眼试管（约 1/4）。\n" +
               $"{Done(StoryWorld.DownedDosu)}音忍·多斯 —— 预选赛，中央塔大厅。之后用音忍的对战牌（铁锭或铅锭、暗影鳞片或组织样本，铁砧）在大厅召唤。\n" +
               $"{Done(StoryWorld.DownedGaara)}我爱罗 —— 骷髅王后（或生命 ≥ 400）。正式赛，考试会场；之后用砂隐的葫芦（沙块、骨头，铁砧）在会场召唤。每个角色首杀必掉八门遁甲之卷与小李的负重护腿。\n" +
               $"{Done(StoryWorld.DownedNeji)}日向宁次 —— 正式赛开始后，用卡卡西转交的切磋书在会场召唤。掉白眼（约 1/4）。";
    }

    private static string JutsuText()
    {
        SubstitutionPlayer substitution = Main.LocalPlayer.GetModPlayer<SubstitutionPlayer>();
        float regen = Main.LocalPlayer.GetModPlayer<StyleCorePlayer>().LogRegenTicks / 60f;
        return "替身术（基础忍术，所有职业可用）\n" +
               $"· 你身上备着几根替身木头（查克拉条下方的木头图标，开局 {ChakraRules.StartingLogs} 根）\n" +
               "· 被敌人打中时自动用掉一根：你换到旁边，原地落下木头，这一击无效；不需要按键\n" +
               $"· 木头每 {regen:0} 秒恢复一根，每打中敌人一次恢复得更快\n" +
               "· 替身之后会潜伏 2 秒：下一击必定暴击，伤害更高\n" +
               $"· 按【{ShinobiKeybinds.SubstitutionKeyName()}】主动替身：用一根木头和 {ChakraRules.SubstitutionCost} 查克拉，朝移动方向瞬移并潜伏\n" +
               "· 被沙子裹住、被杀气震住时，按这个键用一根木头挣脱\n" +
               "· 点穴满三层时，木头也会被封住\n" +
               "· 想练习按键时机，可以找卡卡西点“练习替身术”\n\n" +
               $"当前：木头 {substitution.Logs}/{substitution.MaxLogs}";
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
        return $"波之国 · 再不斩与白：已收集 {seen}/{RewardCollectionPlayer.WaveRewards.Length}\n\n" +
               "· 每次击败必得一把职业武器（近战、远程、魔法、召唤各一把），有时会多给一把\n" +
               "· 两件面具时装各约 1/7 掉落\n" +
               "· 每个角色第一次击败时，另得查克拉结晶与波之国功绩牌\n" +
               "· 专家模式下改为每人一个宝藏袋";
    }

    private static IEnumerable<HandbookIconGrid.Entry> PathIcons()
    {
        foreach ((string icon, string path, string ultimate, _) in Paths)
            yield return new HandbookIconGrid.Entry(
                ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>($"ShinobiPrototype/Assets/Handbook/{icon}",
                    ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, path, Unlocked: false, Tiers: 3);
    }

    private static string PathsText()
    {
        string lines = "";
        foreach ((_, string path, string ultimate, string teaser) in Paths)
            lines += $"· {path}：{teaser}终点——{ultimate}\n";
        return "忍道：跨职业的修行路线，一次只能走一条的核心，各有三阶。取得第一阶后，这里会点亮。\n\n" + lines +
               "\n第一阶都在中忍考试到木叶崩溃之间取得。";
    }

    // Records unlocked by beating each story boss; the story is told here rather than in cutscenes.
    private static string LoreText()
    {
        if (!StoryWorld.WaveComplete)
            return "卷宗\n\n（空白）击败剧情中的首领后，这里会记下他们的故事。";
        return "卷宗 · 波之国\n\n" +
               "【鬼人·桃地再不斩】雾隐村的叛忍，“雾隐七人众”之一，佩斩首大刀。据说曾在血雾之里的毕业考核中一人屠尽同届考生，" +
               "此后被称为“鬼人”。政变失败后逃离雾隐，靠做刺客筹集资金，受雇于卡多，前来刺杀造桥工达兹纳。\n\n" +
               "【白】身负冰遁血继限界的少年。血继限界者在战乱的水之国遭到猎杀，白失去了双亲，在雪中被再不斩捡回。" +
               "从那天起，他只为再不斩而活，甘愿成为他的“工具”。在大桥上，他以魔镜冰晶困住木叶的忍者，最后替再不斩挡下了致命一击。\n\n" +
               "【卡多】操纵波之国航运的富商。得知再不斩失手后，他带着浪人赶到桥上，打算连再不斩一起除掉——" +
               "却没想到，失去了白的鬼人，还剩最后一口气。\n\n" +
               "【大桥】达兹纳以镇上众人之力修通了大桥。人们为它取名“鸣人大桥”，纪念那些改变了这个国家的忍者。\n\n" +
               "……听说木叶的中忍考试就要开始了。死亡森林里，据说有蛇出没。";
    }

    private static string ChakraText()
    {
        ChakraPlayer chakra = Main.LocalPlayer.GetModPlayer<ChakraPlayer>();
        return $"查克拉 {chakra.Chakra}/{chakra.MaxChakra}　　查克拉结晶 {chakra.Crystals}/{ChakraRules.MaxCrystals}\n\n" +
               "恢复\n" +
               $"· 自然恢复：每秒 {ChakraRules.SafeRegenPerSecond:0} 点；施术后 {ChakraRules.RegenDelayTicks / 60} 秒内降为每秒 {ChakraRules.CombatRegenPerSecond:0} 点\n" +
               $"· 命中恢复：任何职业的攻击（含召唤物）命中敌人，每击回 {ChakraRules.HitRegenPerHit} 点，每秒最多 {ChakraRules.HitRegenPerSecondCap} 点\n" +
               $"· 兵粮丸：立即回 {ChakraRules.PillRestore} 点，之后 {ChakraRules.PillSicknessTicks / 60} 秒内不能再吃；蘑菇 + 太阳花在工作台合成（一次 2 颗）\n\n" +
               "提升上限\n" +
               $"· 查克拉结晶：地下洞穴中发蓝光的结晶，用镐挖下后使用，每颗 +{ChakraRules.CrystalBonus}，困难模式前最多 {ChakraRules.PreHardmodeMaxChakra}";
    }
}
