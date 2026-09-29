using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Items;

namespace ShinobiPrototype.Content.Commands;

public sealed class M0Command : ModCommand
{
    public override CommandType Type => CommandType.Chat;
    public override string Command => "m0";
    public override string Usage => "/m0 或 /m0 god [on|off]";
    public override string Description => "领取 M0 测试道具，或切换仅限单人的临时测试无敌";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        Player player = caller.Player;
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
}
