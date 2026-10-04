using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Items.Jutsu;

namespace ShinobiPrototype.Common.Systems;

// The seal slots (specs/装备与忍术系统.spec.md), drawn by hand while the inventory is open: a hanging scroll to the
// right of the ammo slots with three slots on it, for 2, 4 and 6 seals (seal-slots-ui-v1). Each takes only a scroll of
// its own count; items move in and out as in any inventory slot. (They were accessory slots, which tModLoader drew
// out of line with the scroll and with a visibility toggle beside each, user 2026-10-03.)
public sealed class SealSlotsUI : ModSystem
{
    private const string Ui = "ShinobiPrototype/Assets/UI/";
    private const float Scale = 0.85f;            // vanilla's inventory scale
    private static readonly Vector2 PanelAt = new(590f, 86f);
    // Inside the scroll art (72x176): slots of 52 start 10 in and 12 down, 50 apart.
    private static readonly Vector2 FirstSlot = new(10f, 12f);
    private const float SlotStep = 50f;

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name == "Vanilla: Inventory");
        if (index < 0)
            return;
        layers.Insert(index + 1, new LegacyGameInterfaceLayer("ShinobiPrototype: Seal Slots", Draw, InterfaceScaleType.UI));
    }

    private static Texture2D Art(string name) =>
        ModContent.HasAsset(Ui + name) ? ModContent.Request<Texture2D>(Ui + name).Value : null;

    private static bool Draw()
    {
        if (!Main.playerInventory || Main.gameMenu || Main.LocalPlayer is not { active: true } player)
            return true;
        SealPlayer seals = player.GetModPlayer<SealPlayer>();
        SpriteBatch sb = Main.spriteBatch;
        if (Art("SealSlotPanel") is { } panel)
            sb.Draw(panel, PanelAt, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);

        for (int i = 0; i < 3; i++)
        {
            int count = 2 + 2 * i;
            Vector2 at = PanelAt + (FirstSlot + new Vector2(0f, i * SlotStep)) * Scale;
            Rectangle box = new((int)at.X, (int)at.Y, (int)(52 * Scale), (int)(52 * Scale));
            ref Item slot = ref seals.Scrolls[i];

            Texture2D back = Art("SealSlotBack") ?? TextureAssets.InventoryBack.Value;
            sb.Draw(back, at, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
            Vector2 centre = at + new Vector2(26f, 26f) * Scale;
            if (!slot.IsAir)
                ItemSlot.DrawItemIcon(slot, ItemSlot.Context.InventoryItem, sb, centre, Scale, 32f, Color.White);
            else if (Art($"SealSlotEmpty_{count}") is { } empty)
                sb.Draw(empty, centre, null, Color.White * 0.8f, 0f, empty.Size() / 2f, Scale, SpriteEffects.None, 0f);

            // Cooling down: the slot darkened from the top by the time left, and the seconds.
            int cooldown = seals.CooldownOf(i);
            if (cooldown > 0 && seals.ScrollFor(count) is { } cooling && cooling.CooldownTicks > 0)
            {
                float left = cooldown / (float)cooling.CooldownTicks;
                sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(box.X, box.Y, box.Width, (int)(box.Height * left)), Color.Black * 0.6f);
                Utils.DrawBorderString(sb, $"{cooldown / 60f:0.0}", centre, Color.White, 0.8f, 0.5f, 0.5f);
            }

            // The key that casts it, in the corner.
            Utils.DrawBorderString(sb, ShinobiKeybinds.SealKeyName(count), at + new Vector2(5f, 3f), new Color(255, 225, 150), 0.7f);

            if (!box.Contains(Main.MouseScreen.ToPoint()) || PlayerInput.IgnoreMouseInterface)
                continue;
            player.mouseInterface = true;
            bool fits = Main.mouseItem.IsAir || Main.mouseItem.ModItem is SealScroll scroll && scroll.Seals == count;
            if (fits)
                ItemSlot.Handle(ref slot, ItemSlot.Context.InventoryItem);
            else
                ItemSlot.MouseHover(ref slot, ItemSlot.Context.InventoryItem);
            if (slot.IsAir)
                Main.hoverItemName = $"{count} 印位：放 {count} 印的结印卷轴，按【{ShinobiKeybinds.SealKeyName(count)}】施展";
        }
        return true;
    }
}
