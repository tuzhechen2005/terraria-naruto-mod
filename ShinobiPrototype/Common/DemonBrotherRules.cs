using System;

namespace ShinobiPrototype.Common;

// The Demon Brothers (M1 spec, stage 5): an elite pair joined by a chain that ambushes the player in rain or sea
// mist once they have found a Mist insignia, before Wave Country is done. They pincer the player from both sides,
// swipe with their claws up close, and every few seconds pull the chain taut and rush past each other so the
// chain sweeps through the player. When one falls the other goes berserk. Kept free of Terraria types for tests.
public static class DemonBrotherRules
{
    public const int LifeMax = 220;
    public const int ContactDamage = 18;
    public const int ChainDamage = 22;
    public const float SpawnChance = 0.02f;
    public const float FlankGap = 110f;
    public const float RunSpeed = 3.4f;
    public const float BerserkSpeedMultiplier = 1.45f;

    public const int SwipeRange = 70;
    public const int SwipeWindupTicks = 20;
    public const int SwipeActiveTicks = 10;
    public const int SwipeRecoveryTicks = 26;

    // Chain sweep: telegraphed by the chain flashing, then both rush past each other at speed.
    public const int ChainCooldownTicks = 300;
    public const int ChainWarnTicks = 36;
    public const int ChainRushTicks = 40;
    public const float ChainRushSpeed = 9f;
    public const float ChainMaxLength = 40 * 16f;
    public const float ChainMinLength = 3 * 16f;

    public static bool SpawnAllowed(bool foundInsignia, bool raining, bool inSeaMist, bool waveComplete,
        bool brothersAlive, bool onSurface) =>
        foundInsignia && (raining || inSeaMist) && !waveComplete && !brothersAlive && onSurface;

    // Until they are first beaten the story waits on them, so that first ambush is certain (user, 2026-09-30): with an
    // insignia found, walking into the bridge's sea mist on the surface brings them after a short warning; if the
    // player flees or dies, again a minute later. Afterwards they only turn up at random, as above.
    public const int AmbushWarnTicks = 120;
    public const int AmbushRetryTicks = 3600;

    public static bool AmbushDue(bool downedBrothers, bool foundInsignia, bool inSeaMist, bool onSurface,
        bool waveComplete, bool brothersAlive, int cooldown) =>
        !downedBrothers && foundInsignia && inSeaMist && onSurface && !waveComplete && !brothersAlive && cooldown <= 0;

    // Each brother holds its own side of the player: -1 left, +1 right.
    public static float FlankX(float playerX, int side) => playerX + side * FlankGap;

    public static bool ChainSweepReady(int cooldown, float brotherGap, bool playerBetween) =>
        cooldown <= 0 && playerBetween && brotherGap >= ChainMinLength && brotherGap <= ChainMaxLength;

    // The chain only hurts while it is being swept, not while it hangs slack.
    public static bool ChainHurts(bool rushing, float length) => rushing && length >= ChainMinLength;
}
