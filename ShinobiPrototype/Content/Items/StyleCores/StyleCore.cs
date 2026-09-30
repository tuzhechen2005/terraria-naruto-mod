using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;

namespace ShinobiPrototype.Content.Items.StyleCores;

// A style core (流派核心): an accessory carrying one school at one tier, with a passive and a chakra technique on the
// style key (specs/流派系统.spec.md). Only one may be worn until the second core slot opens.
public abstract class StyleCore : ModItem
{
    public abstract StyleSchool School { get; }
    public abstract int Tier { get; }
    public virtual int TechniqueCooldown => 60;

    // Chakra for the next press (the Eight Gates close for free).
    public virtual int TechniqueCost(Player player) => 0;

    // The school's passive, applied every tick the core is worn.
    public virtual void ApplyPassive(Player player) { }

    // Called on the local player's client once the key was pressed and chakra and cooldown allowed it.
    public virtual void UseTechnique(Player player) { }

    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 26;
        Item.accessory = true;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(gold: 2);
    }

    // Counts the other cores in the accessory slots (not the slot this one is going into).
    public override bool CanEquipAccessory(Player player, int slot, bool modded)
    {
        int others = 0;
        for (int i = 3; i < 10; i++)
            if (i != slot && player.armor[i].ModItem is StyleCore)
                others++;
        return StyleCoreRules.CanEquip(others, player.GetModPlayer<StyleCorePlayer>().SecondSlot);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        StyleCorePlayer styles = player.GetModPlayer<StyleCorePlayer>();
        styles.Worn ??= this;
        ApplyPassive(player);
    }
}
