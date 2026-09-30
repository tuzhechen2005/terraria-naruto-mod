using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using ShinobiPrototype.Common.Players;

namespace ShinobiPrototype.Common.Systems;

public sealed class ChakraHud : ModSystem
{
    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name == "Vanilla: Resource Bars");
        if (index < 0)
            index = layers.Count;

        layers.Insert(index, new LegacyGameInterfaceLayer(
            "ShinobiPrototype: Chakra",
            DrawChakra,
            InterfaceScaleType.UI));
    }

    private static bool DrawChakra()
    {
        if (Main.gameMenu || Main.LocalPlayer is null || !Main.LocalPlayer.active)
            return true;

        ChakraPlayer player = Main.LocalPlayer.GetModPlayer<ChakraPlayer>();
        ShinobiClientConfig config = ShinobiClientConfig.Instance;
        int x = 20 + config.ChakraBarOffsetX;
        int y = 70 + config.ChakraBarOffsetY;

        Utils.DrawBorderString(Main.spriteBatch, $"查克拉 {player.Chakra}/{player.MaxChakra}", new Vector2(x + 2, y), Color.White, 0.8f);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(x, y + 22, 166, 18), Color.Black * 0.8f);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value,
            new Rectangle(x + 3, y + 25, (int)(160f * player.Chakra / player.MaxChakra), 12), new Color(45, 170, 235));
        int cooldown = Main.LocalPlayer.GetModPlayer<SubstitutionPlayer>().Cooldown;
        Utils.DrawBorderString(Main.spriteBatch,
            cooldown > 0 ? $"替身术 {cooldown / 60f:0.0}s" : $"替身术 [{ShinobiKeybinds.SubstitutionKeyName()}]",
            new Vector2(x + 2, y + 43), cooldown > 0 ? Color.Gray : new Color(200, 170, 110), 0.75f);
        int line = y + 63;
        if (Main.LocalPlayer.GetModPlayer<DebugGodPlayer>().Enabled)
        {
            Utils.DrawBorderString(Main.spriteBatch, "M0 测试无敌", new Vector2(x + 2, line), Color.Gold, 0.75f);
            line += 20;
        }
        if (config.QuestTracker)
            DrawTracker(x + 2, line);
        return true;
    }

    private static void DrawTracker(int x, int y)
    {
        Player player = Main.LocalPlayer;
        (string title, Vector2? where) = player.GetModPlayer<StoryPlayer>().Tracker();
        string text = "任务：" + title;
        if (where is Vector2 at)
        {
            Vector2 d = (at - player.Center) / 16f;
            text += d.Length() < 20f ? "（就在附近）"
                : $"　→ {(d.X < 0 ? "西" : "东")} {System.Math.Abs((int)d.X)} 格" +
                  (System.Math.Abs(d.Y) > 30f ? $"，{(d.Y < 0 ? "上" : "下")} {System.Math.Abs((int)d.Y)} 格" : "");
        }
        Utils.DrawBorderString(Main.spriteBatch, text, new Vector2(x, y), new Color(255, 230, 160), 0.75f);
    }
}
