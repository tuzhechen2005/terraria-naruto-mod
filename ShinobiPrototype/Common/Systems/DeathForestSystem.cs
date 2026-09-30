using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Common.Systems;

// The Rain genin's ambush in the Forest of Death (specs/M2_中忍考试篇.spec.md 3.2): the first time a character hunting
// the other scroll is on the jungle surface, a warning, then three Rain genin close in. If the character escapes or
// falls, the three leave and try again later. Run by the server (or single player) from each player's synced progress.
public sealed class DeathForestSystem : ModSystem
{
    private static int cooldown;
    private static int warnTicks = -1;
    private static int target = -1;

    public override void OnWorldLoad() => Reset();

    public override void OnWorldUnload() => Reset();

    private static void Reset()
    {
        cooldown = 0;
        warnTicks = -1;
        target = -1;
    }

    public override void PostUpdateWorld()
    {
        if (cooldown > 0)
            cooldown--;
        if (warnTicks >= 0)
        {
            if (--warnTicks < 0)
                Spawn();
            return;
        }
        if (Main.GameUpdateCount % 30 != 0 || cooldown > 0)
            return;

        bool rainAlive = NPC.AnyNPCs(ModContent.NPCType<RainGenin>());
        foreach (Player player in Main.ActivePlayers)
        {
            if (player.dead)
                continue;
            ChuninExamPlayer exam = player.GetModPlayer<ChuninExamPlayer>();
            if (!ChuninExamRules.RainAmbushDue(exam.Stage, exam.RainAmbushDone, player.ZoneJungle && player.ZoneOverworldHeight,
                    rainAlive))
                continue;
            target = player.whoAmI;
            warnTicks = ChuninExamRules.RainAmbushWarnTicks;
            cooldown = ChuninExamRules.RainAmbushRetryTicks;
            Tell(player, "林间传来细碎的雨声……头顶的树枝上，有三把伞。");
            return;
        }
    }

    // One on each side, one dropping from the trees.
    private static void Spawn()
    {
        Player player = target >= 0 ? Main.player[target] : null;
        target = -1;
        if (player is not { active: true, dead: false })
        {
            cooldown = 0;
            return;
        }
        int type = ModContent.NPCType<RainGenin>();
        var source = new Terraria.DataStructures.EntitySource_WorldEvent();
        NPC.NewNPC(source, (int)player.Center.X - 22 * 16, (int)player.Bottom.Y, type);
        NPC.NewNPC(source, (int)player.Center.X + 22 * 16, (int)player.Bottom.Y, type);
        NPC.NewNPC(source, (int)player.Center.X + 6 * 16, (int)player.Bottom.Y - 14 * 16, type);
        Tell(player, "雨隐的考生：“你的卷轴，我们收下了。”");
    }

    private static void Tell(Player player, string text)
    {
        Color color = new(170, 190, 255);
        if (Main.netMode == NetmodeID.Server)
            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(text), color, player.whoAmI);
        else
            Main.NewText(text, color);
    }
}
