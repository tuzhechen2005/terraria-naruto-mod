using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Items;

// 白的面具：追杀部队面具（时装）, a boss mask from Zabuza and Haku (about 1 in 7).
[AutoloadEquip(EquipType.Head)]
public sealed class HakuMask : ModItem
{
    public override void SetStaticDefaults()
    {
        ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 24;
        Item.vanity = true;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 75);
    }
}
