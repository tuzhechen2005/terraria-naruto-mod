using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace ShinobiPrototype.Common.Systems;

// The entrance title of a story boss (master spec, "可玩性与引导": a warning first, then the name and who they are in
// large letters, so no boss turns up without the player knowing who it is). Client side only.
public sealed class BossIntroSystem : ModSystem
{
    private const int ShowTicks = 200;
    private const int FadeTicks = 30;

    private static string name;
    private static string title;
    private static int ticks;
    private static uint shownAt;

    // The same name again within half a minute (a group of three, or a boss that re-enters) is not shown twice.
    public static void Show(string bossName, string bossTitle)
    {
        if (Main.dedServ || name == bossName && Main.GameUpdateCount - shownAt < 1800)
            return;
        shownAt = Main.GameUpdateCount;
        name = bossName;
        title = bossTitle;
        ticks = ShowTicks;
    }

    public override void OnWorldUnload() => ticks = 0;

    public override void PostUpdateEverything()
    {
        if (ticks > 0)
            ticks--;
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name == "Vanilla: Inventory");
        layers.Insert(index >= 0 ? index : layers.Count, new LegacyGameInterfaceLayer("ShinobiPrototype: Boss Intro", () =>
        {
            if (ticks <= 0)
                return true;
            float alpha = System.Math.Min(1f, System.Math.Min(ticks, ShowTicks - ticks) / (float)FadeTicks);
            Vector2 center = new(Main.screenWidth / 2f, Main.screenHeight * 0.28f);
            Vector2 nameSize = FontAssets.DeathText.Value.MeasureString(name);
            Utils.DrawBorderStringBig(Main.spriteBatch, name, center - new Vector2(nameSize.X / 2f, nameSize.Y / 2f),
                new Color(255, 225, 150) * alpha, 1f);
            Vector2 titleSize = FontAssets.MouseText.Value.MeasureString(title) * 1.1f;
            Utils.DrawBorderString(Main.spriteBatch, title, center + new Vector2(-titleSize.X / 2f, nameSize.Y * 0.55f),
                Color.White * alpha, 1.1f);
            return true;
        }, InterfaceScaleType.UI));
    }
}
