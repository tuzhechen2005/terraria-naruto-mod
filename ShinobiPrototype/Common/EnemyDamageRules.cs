using System;

namespace ShinobiPrototype.Common;

// Every hit an enemy of this mod lands on a player (specs/敌方伤害标准.spec.md, settled with the user 2026-10-04). Each
// number here is what the player takes in normal mode before defence. Vanilla doubles a hostile projectile's damage
// when it hits a player (Projectile.Damage: Main.DamageVar(damage) * 2), so projectiles are spawned with half
// (ProjectileDamage); contact damage and direct hurts get vanilla's mode multiplier (EnemyDamage). Projectile damage is
// the same in every mode, as in vanilla. Kept free of Terraria types so the rule tests can check the whole table.
public enum AttackKind
{
    Normal,     // at most 15% of the baseline life
    Big,        // at most 30%: warned at least BigTelegraphTicks ahead, with a pose or a mark on the ground
    Lingering,  // a hazard on the ground that may hit again (the venom pool): held to the normal cap
}

public readonly record struct EnemyAttack(string Name, int Damage, AttackKind Kind, int Baseline, int TelegraphTicks);

public static class EnemyDamageRules
{
    public const int NormalPercent = 15;
    public const int BigPercent = 30;
    // A big attack is warned at least this long (user, 2026-10-04: 0.4 s, "a touch longer" for the quick ones).
    public const int BigTelegraphTicks = 24;
    // A hitbox that appears at once next to the player (not a projectile seen in flight) shows its swing this long first.
    public const int InstantMeleeTicks = 15;

    public static int Cap(AttackKind kind, int baseline) =>
        baseline * (kind == AttackKind.Big ? BigPercent : NormalPercent) / 100;

    // The damage to spawn a hostile projectile with, so that the player takes `actual` (rounded down, never above).
    public static int ProjectileDamage(int actual) => Math.Max(1, actual / 2);

    // A big attack's windup, shortened by a phase's tempo but never below the big-attack warning.
    public static int BigWindup(float ticks, float tempo = 1f) =>
        Math.Max(BigTelegraphTicks, (int)Math.Ceiling(ticks / Math.Max(0.01f, tempo)));

    // Baselines: the life a player is expected to have at the story gate where the enemy is met.
    public static class Brothers
    {
        public const int Baseline = 100;
        public const int Contact = 15;
        public const int Swipe = 27;
        public const int Chain = 22;
    }

    // The world's first ninja enemies (specs/空档衔接与火影小兵.spec.md, tier one): met before the Eye of Cthulhu.
    public static class Ronin
    {
        public const int Baseline = 100;
        public const int Contact = 14;
        public const int Slash = 15;
        public const int SlashWindupTicks = 20;
    }

    public static class RogueGenin
    {
        public const int Baseline = 100;
        public const int Contact = 10;
        public const int Kunai = 12;
        public const int Stab = 15;
        public const int StabWindupTicks = InstantMeleeTicks;
    }

    public static class WaterClone
    {
        public const int Baseline = 200;
        public const int Contact = 22;
        public const int Slash = 26;
        public const int SlashWindupTicks = 30;
    }

    public static class MirrorCage
    {
        public const int Baseline = 200;
        public const int Boundary = 10;
    }

    public static class Zabuza
    {
        public const int Baseline = 200;
        public const int Slash = 48;
        public const int WaterWave = 40;
        public const int WaterDragon = 60;
        public const int NeedleFan = 28;
        public const int NeedleRain = 28;
        public const int NeedleSpiral = 26;
        public const int Dash = 50;
        public const int DemonDash = 60;
        public const int ThrownSword = 56;   // out and back, each leg may hit once (M10)
        public const int KunaiDash = 20;
    }

    public static class Haku
    {
        public const int Baseline = 200;
        public const int Needle = 28;
        public const int MirrorDash = 42;
        public const int CageFan = 26;
        public const int FrenzyFan = 28;
        public const int ExposedNeedle = 24;
        public const int ThousandNeedles = 38;
    }

    public static class Genin
    {
        public const int Baseline = 300;
        public const int CandidateContact = 26;
        public const int CandidateStrike = 40;
        public const int CandidateStrikeWindupTicks = InstantMeleeTicks;
        public const int CandidateShuriken = 16;
        public const int RainContact = 30;
        public const int RainSenbon = 20;
        public const int RainUmbrella = 20;
        public const int RainUmbrellaHangTicks = 40;   // the shortest hang (ExamSenbon, 40 + i * 6)
    }

    public static class Dosu
    {
        public const int Baseline = 300;
        public const int Drill = 60;          // each of the two thrusts
        public const int SecondThrustTicks = BigTelegraphTicks;
        public const int Wave = 44;
        public const int Quake = 48;
        public const int Ring = 40;
        public const int Leap = 68;
    }

    public static class Orochimaru
    {
        public const int Baseline = 300;
        public const int Barrage = 30;
        public const int GiantSnake = 90;
        public const int SnakeHands = 80;
        public const int HandsWindupTicks = 24;
        public const int WindBlast = 50;
        public const int WindWindupTicks = 27;
        public const int DashBite = 80;
        public const int DashWindupTicks = BigTelegraphTicks;
        public const int NeckBite = 90;
        public const int NeckBiteTicks = 24;   // the coil and the reach before the bite at the mark
        public const int FiveSeal = 58;
        public const int FiveSealWindupTicks = BigTelegraphTicks;
        public const int Swarm = 40;
        public const int SwarmWindupTicks = 20;
        public const int Venom = 40;
        public const int VenomWindupTicks = 24;
        public const int VenomPool = 20;
        public const int SnakeRain = 45;
        public const int SnakeRainWindupTicks = 20;
        public const int SnakeRainHangTicks = 45;      // the first snake hangs this long (OrochimaruBullets, 45 + i * 8)
        public const int Kusanagi = 90;
        public const int KusanagiAimTicks = 34;
    }

    public static class Neji
    {
        public const int Baseline = 400;
        public const int Palm = 50;
        public const int FirstPalmTick = InstantMeleeTicks;
        public const int PalmSpacingTicks = 12;
        public const int Rotation = 52;
        public const int RotationWindupTicks = BigTelegraphTicks;
        public const int SixtyFour = 120;
    }

    public static class Gaara
    {
        public const int Baseline = 400;
        public const int Pellet = 36;
        public const int Quicksand = 64;
        public const int Shuriken = 44;
        public const int ShurikenWindupTicks = 30;
        public const int Burial = 120;
        public const int SandWave = 68;
        public const int SandWaveWindupTicks = 40;
        public const int SandArm = 88;
        public const int SandArmWindupTicks = 36;
        public const int AirBullet = 70;
        public const int AirBulletWindupTicks = 34;
    }
}
