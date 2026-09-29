using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
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
    private enum Page { Mission, Jutsu, Chakra }

    private static readonly (Page Page, string Name)[] Tabs =
    {
        (Page.Mission, "任务"),
        (Page.Jutsu, "忍术"),
        (Page.Chakra, "查克拉"),
    };

    private static readonly Color TabIdle = new Color(63, 82, 151) * 0.85f;
    private static readonly Color TabActive = new(200, 150, 60);

    private readonly List<(Page Page, UITextPanel<string> Panel)> tabPanels = new();
    private UIPanel panel;
    private UIText body;
    private Page page = Page.Mission;
    private int refreshTimer;

    public override void OnInitialize()
    {
        panel = new UIPanel();
        panel.Width.Set(600f, 0f);
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
            UITextPanel<string> tab = new(Tabs[i].Name, 0.9f);
            tab.Width.Set(90f, 0f);
            tab.Left.Set(120f + i * 96f, 0f);
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
        body?.SetText(page switch
        {
            Page.Jutsu => JutsuText(),
            Page.Chakra => ChakraText(),
            _ => MissionText(),
        });
    }

    private static string MissionText()
    {
        Player player = Main.LocalPlayer;
        int insignia = System.Math.Min(3, player.CountItem(ModContent.ItemType<MistInsignia>()));
        return player.GetModPlayer<StoryPlayer>().CurrentObjective() + "\n\n" +
               "进度\n" +
               $"· 雾隐标记：{insignia}/3（雾隐侦察兵出没于远离出生点的地表，海边更多）\n" +
               $"· 再不斩与白：{(StoryWorld.WaveComplete ? "已击败" : "未击败")}\n" +
               $"· 海边大桥：{(WaveBridgeWorld.Site is null ? "这个世界还没有（找卡卡西要施工图）" : WaveBridgeWorld.Finished ? "已完工" : "未完工，桥头有造桥工达兹纳")}\n" +
               $"· 中忍考试：{(StoryWorld.WaveComplete ? "已开放" : "击败再不斩与白后开放")}\n\n" +
               "不知道下一步做什么，可以去问卡卡西。";
    }

    private static string JutsuText()
    {
        SubstitutionPlayer substitution = Main.LocalPlayer.GetModPlayer<SubstitutionPlayer>();
        string status = substitution.Cooldown > 0 ? $"冷却 {substitution.Cooldown / 60f:0.0} 秒" : "就绪";
        return "替身术（基础忍术，所有职业可用）\n" +
               $"· 按键：【{ShinobiKeybinds.SubstitutionKeyName()}】（可在“设置 → 控制”中修改）\n" +
               $"· 消耗 {ChakraRules.SubstitutionCost} 查克拉；按下后 {ChakraRules.SubstitutionWindowTicks / 60f:0.0} 秒内受到的攻击会被完全闪避\n" +
               "· 成功时原地留下一截木头，你出现在附近的安全位置，并短暂无敌\n" +
               $"· 无论成功与否，冷却 {ChakraRules.SubstitutionCooldownTicks / 60} 秒\n" +
               "· 诀窍：看准敌人出手的瞬间再按；按早了会白白浪费查克拉\n" +
               "· 想练习，可以找卡卡西点“练习替身术”\n\n" +
               $"状态：{(substitution.Mastered ? "已掌握" : "尚未成功施展过")}　当前：{status}";
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
