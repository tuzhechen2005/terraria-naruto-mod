using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Items.Weapons;

// 斩首大刀 (melee, from Zabuza and Haku): a slow, heavy swing that hits hard and knocks back far. Like the
// original blade, which mends itself with the blood of those it cuts, hits sometimes restore a little life.
// First pass (2026-09-29), to be tuned with the user.
public sealed class Kubikiribocho : ModItem
{
    private const int HealChanceOneIn = 5;
    private const int Heal = 2;

    public override void SetDefaults()
    {
        Item.damage = 26;
        Item.DamageType = DamageClass.Melee;
        Item.width = 56;
        Item.height = 56;
        Item.scale = 1.2f;
        Item.useTime = 32;
        Item.useAnimation = 32;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 7.5f;
        Item.autoReuse = true;
        Item.UseSound = SoundID.Item1;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(gold: 1);
    }

    public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!target.immortal && Main.rand.NextBool(HealChanceOneIn))
            player.Heal(Heal);
    }
}
