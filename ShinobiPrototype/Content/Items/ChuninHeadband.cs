using Terraria;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Items;

public sealed class ChuninHeadband : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 18;
        Item.accessory = true;
        Item.rare = Terraria.ID.ItemRarityID.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.statDefense += 3;
        player.GetDamage(DamageClass.Melee) += 0.04f;
        player.GetDamage(DamageClass.Magic) += 0.04f;
    }
}
