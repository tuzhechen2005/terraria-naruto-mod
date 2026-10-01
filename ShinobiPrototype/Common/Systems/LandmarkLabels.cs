using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;

namespace ShinobiPrototype.Common.Systems;

// The village's and the exams' landmarks are easy to find (user, 2026-09-30: the Academy and Ichiraku were hard to
// find with only a small sign): a large name floats over each one's roof as the player comes near, and each has a
// marker on the full map with its name on hover.
public static class Landmarks
{
    public readonly record struct Landmark(string Name, Vector2 RoofWorld);

    private static readonly string[] VillageNames = { "阿吽大门", "火影楼", "忍者学校", "一乐拉面", "木叶医院", "第三演习场" };

    public static IEnumerable<Landmark> All()
    {
        if (KonohaWorld.Site is KonohaSite site)
            foreach (KBuilding building in KonohaWorld.Design.Buildings)
                if (System.Array.IndexOf(VillageNames, building.Name) >= 0)
                    yield return new Landmark(building.Name,
                        new Vector2((site.X((building.X0 + building.X1) / 2) + 0.5f) * 16f, site.Y(building.Top) * 16f));
        foreach (ExamSite exam in ExamSiteWorld.All())
        {
            ExamSiteDesign design = ExamSiteWorld.Design(exam.Kind, exam.Dir);
            foreach (KBuilding building in design.Buildings)
                yield return new Landmark(building.Name,
                    new Vector2((exam.CenterX + (building.X0 + building.X1) / 2 + 0.5f) * 16f, (exam.GroundY + building.Top) * 16f));
        }
    }
}

public sealed class LandmarkLabelSystem : ModSystem
{
    private const float ShowWithinTiles = 50f;
    private const float FullWithinTiles = 35f;

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name == "Vanilla: Entity Health Bars");
        layers.Insert(index >= 0 ? index : 0, new LegacyGameInterfaceLayer("ShinobiPrototype: Landmark Labels", () =>
        {
            if (Main.gameMenu || Main.LocalPlayer is not { active: true } player)
                return true;
            foreach (Landmarks.Landmark landmark in Landmarks.All())
            {
                float tiles = System.Math.Abs(landmark.RoofWorld.X - player.Center.X) / 16f;
                if (tiles > ShowWithinTiles || System.Math.Abs(landmark.RoofWorld.Y - player.Center.Y) / 16f > 60f)
                    continue;
                float alpha = tiles <= FullWithinTiles ? 1f : 1f - (tiles - FullWithinTiles) / (ShowWithinTiles - FullWithinTiles);
                Vector2 size = FontAssets.DeathText.Value.MeasureString(landmark.Name) * 0.55f;
                Vector2 at = landmark.RoofWorld - Main.screenPosition - new Vector2(size.X / 2f, size.Y + 18f);
                Utils.DrawBorderStringBig(Main.spriteBatch, landmark.Name, at, new Color(255, 230, 170) * alpha, 0.55f);
            }
            return true;
        }, InterfaceScaleType.Game));
    }
}

public sealed class LandmarkMapLayer : ModMapLayer
{
    public override void Draw(ref MapOverlayDrawContext context, ref string text)
    {
        var sign = TextureAssets.Item[ItemID.Sign];
        Main.instance.LoadItem(ItemID.Sign);
        foreach (Landmarks.Landmark landmark in Landmarks.All())
            if (context.Draw(sign.Value, landmark.RoofWorld / 16f, Color.White, new SpriteFrame(1, 1), 1f, 1.4f, Alignment.Bottom)
                .IsMouseOver)
                text = landmark.Name;
    }
}
