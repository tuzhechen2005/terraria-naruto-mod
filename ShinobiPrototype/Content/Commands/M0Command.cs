using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;

namespace ShinobiPrototype.Content.Commands;

public sealed class M0Command : ModCommand
{
    public override CommandType Type => CommandType.Chat;
    public override string Command => "m0";
    public override string Usage => "/m0、/m0 items、/m0 time <day|noon|night|midnight|hh:mm>、/m0 bridge、/m0 preview、/m0 sighting、/m0 senbon 或 /m0 god [on|off]";
    public override string Description => "领取 M0 测试道具、领取模组全部物品（含开发者之翼），或切换仅限单人的临时测试无敌";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        Player player = caller.Player;
        if (args.Length == 1 && args[0].Equals("items", StringComparison.OrdinalIgnoreCase))
        {
            int given = GiveAllItems(player);
            caller.Reply($"已发放本模组全部 {given} 种物品（含开发者之翼）。", Color.LightGreen);
            return;
        }

        if (args.Length == 1 && args[0].Equals("bridge", StringComparison.OrdinalIgnoreCase))
        {
            if (WaveBridgeWorld.Site is not BridgeSite site)
            {
                caller.Reply("这个世界还没有大桥。", Color.OrangeRed);
                return;
            }
            player.Teleport(new Vector2(site.X(0) * 16f, (site.DeckY - 3) * 16f), TeleportationStyleID.RodOfDiscord);
            int built = WaveBridgeWorld.BuiltVersion;
            caller.Reply($"已传送到桥头。本世界大桥版本：{(built > 0 ? $"v{built}" : "未记录（很旧）")}，当前代码：v{BridgeDesign.Version}" +
                (built == BridgeDesign.Version ? "。" : "——需要新建世界才能看到最新的大桥和小屋。"),
                built == BridgeDesign.Version ? Color.LightGreen : Color.Orange);
            return;
        }

        if (args.Length == 1 && args[0].Equals("preview", StringComparison.OrdinalIgnoreCase))
        {
            if (!WaveBridgeWorld.Site.HasValue)
            {
                caller.Reply("这个世界还没有大桥。", Color.OrangeRed);
                return;
            }
            MistPreviewSystem.Play();
            caller.Reply("在本机重放迷雾预告（不改变世界进度）。", Color.LightGreen);
            return;
        }

        if (args.Length == 1 && args[0].Equals("sighting", StringComparison.OrdinalIgnoreCase))
        {
            MistSightingSystem.StartSighting(WaveBridgeWorld.Site, player, withWhisper: true);
            caller.Reply("在本机播放一次雾中剪影出没（不改变进度）。在断口附近按正式位置出现，否则出现在你面前约 15 格。", Color.LightGreen);
            return;
        }

        if (args.Length == 1 && args[0].Equals("senbon", StringComparison.OrdinalIgnoreCase))
        {
            player.GetModPlayer<MistEncounterPlayer>().ThrowWarning(WaveBridgeWorld.Site);
            caller.Reply("在本机播放一次千本警告（不改变进度）。在断口附近按正式位置出现，否则从你面前约 15 格处飞来。", Color.LightGreen);
            return;
        }

        if (args.Length > 0 && args[0].Equals("time", StringComparison.OrdinalIgnoreCase))
        {
            SetTime(caller, args);
            return;
        }

        if (args.Length > 0)
        {
            if (!args[0].Equals("god", StringComparison.OrdinalIgnoreCase) || args.Length > 2)
            {
                caller.Reply(Usage, Color.OrangeRed);
                return;
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                caller.Reply("测试无敌只在单人模式可用。", Color.OrangeRed);
                return;
            }

            DebugGodPlayer debug = player.GetModPlayer<DebugGodPlayer>();
            if (!DebugModeRules.TryResolveGodMode(debug.Enabled,
                args.Length == 2 ? args[1] : null, out bool enabled))
            {
                caller.Reply(Usage, Color.OrangeRed);
                return;
            }

            debug.SetEnabled(enabled);
            caller.Reply(enabled
                ? "测试无敌已开启：伤害将被完全忽略。输入 /m0 god off 可关闭。"
                : "测试无敌已关闭：伤害恢复正常。", enabled ? Color.LightGreen : Color.Orange);
            return;
        }

        player.QuickSpawnItem(player.GetSource_Misc("ShinobiM0"), ModContent.ItemType<TrainingKunai>());
        player.QuickSpawnItem(player.GetSource_Misc("ShinobiM0"), ModContent.ItemType<TrainingRasengan>());
        player.QuickSpawnItem(player.GetSource_Misc("ShinobiM0"), ModContent.ItemType<M0ZabuzaChallengeScroll>());
    }

    // Every item this mod adds, for in-game testing. Legacy items that turn into their replacements are skipped.
    private int GiveAllItems(Player player)
    {
        int given = 0;
        foreach (ModItem item in Mod.GetContent<ModItem>())
        {
            if (item is MissionScroll or HakuChallengeScroll)
                continue;

            int stack = item switch
            {
                ChakraCrystal => ChakraRules.MaxCrystals,
                ChakraPill => 20,
                MistInsignia => 3,
                _ => 1,
            };
            player.QuickSpawnItem(player.GetSource_Misc("ShinobiM0"), item.Type, stack);
            given++;
        }
        return given;
    }

    private static void SetTime(CommandCaller caller, string[] args)
    {
        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            caller.Reply("调整时间只在单人模式可用。", Color.OrangeRed);
            return;
        }
        if (args.Length != 2 || !DebugModeRules.TryParseTime(args[1], out bool dayTime, out double time))
        {
            caller.Reply("用法：/m0 time day（4:30）| noon（12:00）| night（19:30）| midnight（0:00）| hh:mm（24 小时制）", Color.OrangeRed);
            return;
        }

        Main.dayTime = dayTime;
        Main.time = time;
        caller.Reply($"时间已调整为 {args[1]}（{(dayTime ? "白天" : "夜晚")}）。", Color.LightGreen);
    }
}
