using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.Items.NinjaTools;

namespace ShinobiPrototype.Common.Players;

// The ninja tools' own stealth (ChakraRules.TickToolStealth): it builds while a tool is in hand and empties when a hit
// gets through. Full, or hidden after a blink, the next throw is a stealth throw (NinjaTool).
public sealed class ToolStealthPlayer : ModPlayer
{
    public float Meter { get; private set; }
    public bool HoldingTool => Player.HeldItem?.ModItem is NinjaTool;
    public bool Ready => Meter >= 1f || Player.GetModPlayer<StealthPlayer>().Hidden;

    public override void PostUpdate()
    {
        bool wasFull = Meter >= 1f;
        Meter = ChakraRules.TickToolStealth(Meter, HoldingTool);
        if (!wasFull && Meter >= 1f && Player.whoAmI == Main.myPlayer)
        {
            SoundEngine.PlaySound(SoundID.MaxMana with { Pitch = -0.3f }, Player.Center);
            for (int i = 0; i < 12; i++)
                Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2CircularEdge(18f, 24f), DustID.PurpleTorch,
                    Vector2.Zero, 0, default, 1.2f).noGravity = true;
        }
        if (Meter >= 1f && HoldingTool && Main.rand.NextBool(8))
            Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2Circular(14f, 20f), DustID.PurpleTorch,
                new Vector2(0f, -0.5f), 0, default, 0.8f).noGravity = true;
    }

    // Spends the stealth on a throw: true if there was any.
    public bool TakeStealthThrow()
    {
        if (!Ready)
            return false;
        Meter = 0f;
        return true;
    }

    public override void OnHurt(Player.HurtInfo info) => Meter = 0f;

    public override void OnRespawn() => Meter = 0f;
}
