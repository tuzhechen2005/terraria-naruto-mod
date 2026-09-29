using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;

namespace ShinobiPrototype.Content.Items;

public sealed class TrainingKunai : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 16;
        Item.DamageType = DamageClass.Melee;
        Item.width = 18;
        Item.height = 18;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 2f;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;
        Item.rare = ItemRarityID.White;
    }

    public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
    {
        player.GetModPlayer<ChakraPlayer>().Restore(5);
        player.GetModPlayer<StoryPlayer>().RegisterKunaiHit(target);
    }
}
