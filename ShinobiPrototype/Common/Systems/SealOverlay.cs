using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Items.Jutsu;

namespace ShinobiPrototype.Common.Systems;

// While the local player forms hand seals: the zodiac signs of the scroll over their head, lit as each seal forms, and
// its name under them.
public sealed class SealOverlay : ModSystem
{
    private const string Fallback = "子丑寅卯辰巳";

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name == "Vanilla: Entity Health Bars");
        layers.Insert(index < 0 ? 0 : index, new LegacyGameInterfaceLayer("ShinobiPrototype: Hand Seals", Draw, InterfaceScaleType.Game));
    }

    private static bool Draw()
    {
        Player player = Main.LocalPlayer;
        if (Main.gameMenu || player is not { active: true } || player.dead)
            return true;
        SealPlayer seals = player.GetModPlayer<SealPlayer>();
        if (!seals.Weaving)
            return true;
        int formed = seals.Seals;
        SealScroll forming = seals.Forming;
        string signs = forming?.Signs ?? Fallback;
        Vector2 top = player.Top - Main.screenPosition - new Vector2(0f, 40f);
        const float step = 22f;
        float x = top.X - (signs.Length - 1) * step / 2f;
        for (int i = 0; i < signs.Length; i++)
        {
            bool done = i < formed;
            Utils.DrawBorderString(Main.spriteBatch, signs[i].ToString(), new Vector2(x + i * step, top.Y),
                done ? new Color(140, 210, 255) : new Color(90, 100, 120) * 0.7f, done ? 1f : 0.85f, 0.5f, 0.5f);
        }
        if (forming != null)
            Utils.DrawBorderString(Main.spriteBatch, forming.Item.Name, top + new Vector2(0f, 22f), new Color(255, 225, 150), 0.75f, 0.5f, 0.5f);
        return true;
    }
}
