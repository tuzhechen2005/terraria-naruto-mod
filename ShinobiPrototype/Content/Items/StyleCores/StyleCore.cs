using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;

namespace ShinobiPrototype.Content.Items.StyleCores;

// A style core (流派核心): an accessory carrying one school at one tier, with a passive and a chakra technique on the
// style key. Only one may be worn. The concrete cores wait on specs/流派系统.spec.md being confirmed.
public abstract class StyleCore : ModItem
{
    public abstract StyleSchool School { get; }
    public abstract int Tier { get; }
    public virtual int TechniqueCost => 0;
    public virtual int TechniqueCooldown => 60;

    // The school's passive, applied every tick the core is worn.
    public virtual void ApplyPassive(Player player) { }

    // Called on the local player's client once the key was pressed and chakra and cooldown allowed it.
    public virtual void UseTechnique(Player player) { }

    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 24;
        Item.accessory = true;
        Item.rare = ItemRarityID.Orange;
    }

    public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player) =>
        StyleCoreRules.CanEquip(equippedItem.ModItem is StyleCore && incomingItem.ModItem is StyleCore);

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<StyleCorePlayer>().Worn = this;
        ApplyPassive(player);
    }
}
