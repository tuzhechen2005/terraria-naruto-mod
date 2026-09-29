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
        if (Main.LocalPlayer.GetModPlayer<DebugGodPlayer>().Enabled)
            Utils.DrawBorderString(Main.spriteBatch, "M0 测试无敌", new Vector2(x + 2, y + 63), Color.Gold, 0.75f);
        return true;
    }
}
