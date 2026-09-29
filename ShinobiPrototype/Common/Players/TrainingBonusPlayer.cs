using Terraria;
using Terraria.ModLoader;
using ShinobiPrototype.Content.Items;

namespace ShinobiPrototype.Common.Players;

public sealed class TrainingBonusPlayer : ModPlayer
{
    public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
    {
        int style = Player.GetModPlayer<StoryPlayer>().CombatStyle;
        if (item.type == ModContent.ItemType<TrainingKunai>() && style == 1)
            damage *= 1.15f;
        if ((item.type == ModContent.ItemType<ChakraPalm>() || item.type == ModContent.ItemType<ElementalTechnique>()) && style == 2)
            damage *= 1.15f;
        if (style == 3 && (item.type == ModContent.ItemType<TrainingKunai>() ||
                           item.type == ModContent.ItemType<ChakraPalm>() || item.type == ModContent.ItemType<ElementalTechnique>()))
            damage *= 1.07f;
    }
}
