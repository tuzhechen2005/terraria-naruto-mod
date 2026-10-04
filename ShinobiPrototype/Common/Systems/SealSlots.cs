using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Items.Jutsu;

namespace ShinobiPrototype.Common.Systems;

// The three seal slots beside the accessories (specs/装备与忍术系统.spec.md): each takes one scroll of its own number
// of seals; the scroll in it is what goes off when the seal key is let go at that count. They stand in a column of
// their own to the right of the ammo slots (user, 2026-10-03: below the accessories the 6-seal slot was off screen).
public abstract class SealSlot : ModAccessorySlot
{
    protected abstract int Seals { get; }

    public override Vector2? CustomLocation => new Vector2(586f, 104f + SealRules.SlotIndex(Seals) * 50f);

    // The number of seals written beside the slot.
    public override bool PreDraw(AccessorySlotType context, Item item, Vector2 position, bool isHovered)
    {
        if (context == AccessorySlotType.FunctionalSlot)
            Utils.DrawBorderString(Main.spriteBatch, $"{Seals}印", position + new Vector2(48f, 12f), new Color(140, 210, 255), 0.8f);
        return true;
    }

    public override bool DrawVanitySlot => false;
    public override bool DrawDyeSlot => false;

    public override bool CanAcceptItem(Item checkItem, AccessorySlotType context) =>
        context == AccessorySlotType.FunctionalSlot && checkItem.ModItem is SealScroll scroll && scroll.Seals == Seals;

    public override bool ModifyDefaultSwapSlot(Item item, int accSlotToSwapTo) =>
        item.ModItem is SealScroll scroll && scroll.Seals == Seals;

    // The scroll only registers itself; it has no other effect while worn.
    public override void ApplyEquipEffects()
    {
        if (!FunctionalItem.IsAir && FunctionalItem.ModItem is SealScroll)
            Player.GetModPlayer<SealPlayer>().Equip(Seals, FunctionalItem);
    }

    public override void OnMouseHover(AccessorySlotType context) => Main.hoverItemName = $"{Seals} 印位";
}

public sealed class SealSlot2 : SealSlot
{
    protected override int Seals => 2;
}

public sealed class SealSlot4 : SealSlot
{
    protected override int Seals => 4;
}

public sealed class SealSlot6 : SealSlot
{
    protected override int Seals => 6;
}
