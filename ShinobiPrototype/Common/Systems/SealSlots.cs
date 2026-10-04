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

    private const string Ui = "ShinobiPrototype/Assets/UI/";

    private static bool HasArt(string name) => ModContent.HasAsset(Ui + name);

    // Their own look (seal-slots-ui-v1): an indigo slot with a vermilion frame, a faint hand seal and number when
    // empty, and a scroll behind the three of them. Until the art is in, the vanilla slot with the number beside it.
    public override string FunctionalBackgroundTexture => HasArt("SealSlotBack") ? Ui + "SealSlotBack" : base.FunctionalBackgroundTexture;

    public override string FunctionalTexture => HasArt($"SealSlotEmpty_{Seals}") ? Ui + $"SealSlotEmpty_{Seals}" : base.FunctionalTexture;

    public override bool PreDraw(AccessorySlotType context, Item item, Vector2 position, bool isHovered)
    {
        if (context != AccessorySlotType.FunctionalSlot)
            return true;
        // The scroll behind the column, drawn under the first slot (the other two are drawn after it).
        if (Seals == 2 && HasArt("SealSlotPanel"))
        {
            var panel = ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(Ui + "SealSlotPanel").Value;
            float scale = Main.inventoryScale;
            Main.spriteBatch.Draw(panel, position + new Vector2((52f - panel.Width) / 2f, -12f) * scale, null, Color.White, 0f,
                Vector2.Zero, scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }
        if (!HasArt($"SealSlotEmpty_{Seals}"))
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
