using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Items.Jutsu;

namespace ShinobiPrototype.Common.Systems;

// While the local player forms hand seals: the seals formed so far over their head (the zodiac signs of the scroll
// being worked towards, the ones still to come dim), and under them the jutsu that would go off if the key were let go.
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
        SealScroll toward = seals.ScrollFor(SealRules.Toward(formed));
        string signs = toward?.Signs ?? Fallback.Substring(0, SealRules.Toward(formed));
        Vector2 top = player.Top - Main.screenPosition - new Vector2(0f, 40f);
        const float step = 22f;
        float x = top.X - (signs.Length - 1) * step / 2f;
        for (int i = 0; i < signs.Length; i++)
        {
            bool done = i < formed;
            Utils.DrawBorderString(Main.spriteBatch, signs[i].ToString(), new Vector2(x + i * step, top.Y),
                done ? new Color(140, 210, 255) : new Color(90, 100, 120) * 0.7f, done ? 1f : 0.85f, 0.5f, 0.5f);
        }
        SealScroll ready = seals.Ready;
        string under = ready != null ? $"→ {ready.Item.Name}" : formed < 2 ? "" : "（这个印位没有卷轴）";
        if (under.Length > 0)
            Utils.DrawBorderString(Main.spriteBatch, under, top + new Vector2(0f, 22f),
                ready != null ? new Color(255, 225, 150) : Color.Gray, 0.75f, 0.5f, 0.5f);
        return true;
    }
}
