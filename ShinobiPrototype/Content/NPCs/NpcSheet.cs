using System;
using Terraria;
using Terraria.ID;

namespace ShinobiPrototype.Content.NPCs;

// Frame layout shared by the mod's town NPCs built with scripts/build_npc_sheet.py: 56x56 frames stacked
// vertically, facing left: Idle, Walk x6, Jump, Sit, Throw x3.
internal static class NpcSheet
{
    public const int FrameCount = 12;
    public const int IdleFrame = 0;
    public const int WalkFirst = 1;
    public const int WalkFrames = 6;
    public const int JumpFrame = 7;
    public const int SitFrame = 8;
    public const int ThrowFirst = 9;
    public const int ThrowFrames = 3;

    // Every story NPC stands as tall on screen as Ibiki, his 50-pixel body at 1.25 (user, 2026-10-01: Kakashi looked
    // small beside him). Pass the idle frame's body height in pixels; drawing stays anchored at the feet.
    public const float StoryHeight = 50f * 1.25f;
    // Kakashi's sheet (kakashi-direct-pixel-anim, skills/terraria-npc-pixel-art) is drawn at its final size; the water
    // prison and the proctors' stand-in use the same scale.
    public const float KakashiScale = 1f;

    public static float ScaleFor(int bodyPixels) => StoryHeight / bodyPixels;

    // Vanilla town AI states used for animation.
    private const float SittingState = 5f;
    private const float ThrowingState = 10f;

    public static void Animate(NPC npc, int frameHeight)
    {
        // A custom FindFrame skips vanilla's town framing, which is also what turns the sprite to face its way.
        npc.spriteDirection = npc.direction;
        int frame;
        if (npc.ai[0] == SittingState)
            frame = SitFrame;
        else if (npc.ai[0] == ThrowingState)
        {
            float progress = 1f - npc.ai[1] / Math.Max(1, NPCID.Sets.AttackTime[npc.type]);
            frame = ThrowFirst + Math.Clamp((int)(progress * ThrowFrames), 0, ThrowFrames - 1);
        }
        else if (npc.velocity.Y != 0f)
            frame = JumpFrame;
        else if (Math.Abs(npc.velocity.X) > 0.1f)
        {
            npc.frameCounter += Math.Abs(npc.velocity.X);
            frame = WalkFirst + (int)(npc.frameCounter / 8.0) % WalkFrames;
        }
        else
        {
            npc.frameCounter = 0;
            frame = IdleFrame;
        }
        npc.frame.Y = frame * frameHeight;
    }
}
