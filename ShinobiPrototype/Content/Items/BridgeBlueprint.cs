using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Systems;

namespace ShinobiPrototype.Content.Items;

// 达兹纳的施工图: for worlds generated without the bridge. Kakashi hands it out. Used on a beach facing the sea:
// the first use outlines the bridge, a second use in the same place builds it and uses up the blueprint.
public sealed class BridgeBlueprint : ModItem
{
    public override string Texture => "ShinobiPrototype/Content/Items/MissionScroll";

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 28;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.rare = ItemRarityID.Blue;
        Item.UseSound = SoundID.Item64;
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI != Main.myPlayer)
            return true;

        if (WaveBridgeWorld.Site.HasValue)
        {
            Main.NewText("这个世界已经有大桥了。", 250, 200, 120);
            return true;
        }
        if (!BridgeBuilder.TryPlanFromPlayer(player, out BridgeSite site, out string reason))
        {
            Main.NewText(reason, 250, 150, 100);
            return true;
        }
        if (!BridgeBuilder.CheckObstacles(site, out reason))
        {
            BridgeBlueprintNet.ShowOutline(site, blocked: true);
            Main.NewText(reason, 250, 150, 100);
            return true;
        }
        if (!BridgeBlueprintNet.IsConfirming(site))
        {
            BridgeBlueprintNet.ShowOutline(site, blocked: false);
            Main.NewText("施工图上画出了大桥的轮廓（绿色）。确认没问题的话，站在原地再用一次施工图开始建造。", 150, 220, 255);
            return true;
        }

        BridgeBlueprintNet.RequestBuild(site);
        if (--Item.stack <= 0)
            Item.TurnToAir();
        return true;
    }
}
