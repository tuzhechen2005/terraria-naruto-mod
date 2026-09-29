using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Players;

// Flight for the developer wings: in the air, direction keys (jump or up to rise, down to sink) move freely with
// no flight time limit, accelerating while held; with no key held the player hovers. On the ground, walking is untouched.
public sealed class DevFlightPlayer : ModPlayer
{
    private const float BaseSpeed = 8f;
    private const float Acceleration = 0.5f;
    private const float MaxSpeed = 48f;

    private float speed = BaseSpeed;

    public bool Equipped { get; set; }

    public override void ResetEffects() => Equipped = false;

    public override void PreUpdateMovement()
    {
        if (!Equipped || Player.mount.Active || Player.grappling[0] >= 0)
            return;

        Vector2 input = new(
            (Player.controlRight ? 1 : 0) - (Player.controlLeft ? 1 : 0),
            (Player.controlDown ? 1 : 0) - (Player.controlJump || Player.controlUp ? 1 : 0));
        bool grounded = Player.velocity.Y == 0f &&
                        Collision.SolidCollision(Player.BottomLeft, Player.width, 2, acceptTopSurfaces: true);
        if (grounded && input.Y >= 0f)
        {
            speed = BaseSpeed;
            return;
        }

        if (input == Vector2.Zero)
        {
            speed = BaseSpeed;
            Player.velocity = Vector2.Zero;
        }
        else
        {
            speed = System.Math.Min(MaxSpeed, speed + Acceleration);
            Player.velocity = Vector2.Normalize(input) * speed;
            if (speed > 20f && Main.rand.NextBool(2))
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.Cloud, 0f, 0f, 150, default, 1.1f);
        }
        Player.fallStart = (int)(Player.position.Y / 16f);
        Player.gravity = 0f;
    }
}
