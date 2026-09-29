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
        Rectangle background = new(20, 92, 166, 18);
        Rectangle fill = new(23, 95, (int)(160f * player.Chakra / ChakraPlayer.MaximumChakra), 12);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, background, Color.Black * 0.8f);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, fill, new Color(45, 170, 235));
        Utils.DrawBorderString(Main.spriteBatch, $"查克拉 {player.Chakra}/{ChakraPlayer.MaximumChakra}", new Vector2(22, 70), Color.White, 0.8f);
        if (Main.LocalPlayer.GetModPlayer<DebugGodPlayer>().Enabled)
            Utils.DrawBorderString(Main.spriteBatch, "M0 测试无敌", new Vector2(22, 113), Color.Gold, 0.75f);
        return true;
    }
}
