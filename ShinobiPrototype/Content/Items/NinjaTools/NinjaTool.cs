using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;

namespace ShinobiPrototype.Content.Items.NinjaTools;

// A ninja tool (specs/装备与忍术系统.spec.md): a thrown ranged weapon that builds its own stealth while held
// (ToolStealthPlayer). When the meter is full, or the player is hidden after a blink, the next throw is the tool's own
// stealth throw instead of the plain one, and the meter starts again.
public abstract class NinjaTool : ModItem
{
    // The stealth throw; the normal throw is the item's own shoot.
    protected abstract void StealthThrow(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity,
        int type, int damage, float knockback);

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type,
        int damage, float knockback)
    {
        ToolStealthPlayer stealth = player.GetModPlayer<ToolStealthPlayer>();
        if (!stealth.TakeStealthThrow())
            return true;
        SoundEngine.PlaySound(SoundID.Item71 with { Pitch = 0.4f, Volume = 0.8f }, player.Center);
        StealthThrow(player, source, position, velocity, type, damage, knockback);
        return false;
    }
}
