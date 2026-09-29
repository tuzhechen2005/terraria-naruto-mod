namespace ShinobiPrototype.Common;

public static class WaveDuoRules
{
    public const int HakuMaxLife = 560;
    public const int HakuNeedleWindupTicks = 36;
    public const int HakuNeedleRecoveryTicks = 38;
    public const int HakuMirrorWindupTicks = 38;
    public const int HakuDashActiveTicks = 17;
    public const int HakuDashRecoveryTicks = 45;
    public const float ZabuzaLastStandDashMultiplier = 1.16f;
    public const int PrismCooldownTicks = 780;
    public const int PrismWindupTicks = 52;
    public const int PrismHiddenTicks = 180;
    public const int PrismRecoverTicks = 44;
    public const int PrismWarningTicks = 28;
    public const int PrismFirstVolleyTick = 22;
    public const int PrismVolleySpacingTicks = 40;
    public const int PrismVolleyCount = 4;
    public const float HakuNeedleDrawScale = 0.055f;
    public const float PrismShardDrawScale = 0.067f;

    public static bool HakuMayAttack(int zabuzaState) =>
        zabuzaState != ZabuzaCombatRules.MistTransition;

    public static bool HakuMayUseStrongAttack(int zabuzaState) => zabuzaState is not
        (ZabuzaCombatRules.MistTransition or ZabuzaCombatRules.RainWindup or
         ZabuzaCombatRules.SpiralWindup or ZabuzaCombatRules.DashWindup or
         ZabuzaCombatRules.DashActive or ZabuzaCombatRules.DashReaim or
         ZabuzaCombatRules.DashChainActive);

    public static int HakuNeedleCount(bool lastStand) => lastStand ? 2 : 1;
    public static float HakuDashSpeed(bool lastStand) => lastStand ? 11.5f : 8.5f;
    public static int HakuDashWindupTicks(bool lastStand) => lastStand ? 26 : 34;
    public static bool PrismMayStart(bool hakuAlive, bool zabuzaAlive,
        int zabuzaState, int elapsedCooldown) => hakuAlive && zabuzaAlive &&
        zabuzaState == ZabuzaCombatRules.Approach && elapsedCooldown >= PrismCooldownTicks;
    public static int PrismVolleyTick(int index) =>
        PrismFirstVolleyTick + index * PrismVolleySpacingTicks;
    public static bool IsPrismVolley(int hiddenTick) => hiddenTick >= PrismFirstVolleyTick &&
        hiddenTick <= PrismVolleyTick(PrismVolleyCount - 1) &&
        (hiddenTick - PrismFirstVolleyTick) % PrismVolleySpacingTicks == 0;
    public static int PrismVolleyIndex(int hiddenTick) =>
        (hiddenTick - PrismFirstVolleyTick) / PrismVolleySpacingTicks;
    public static int SoftenedDamage(int baseDamage) => (baseDamage * 9 + 5) / 10;
    public static bool EncounterComplete(bool hakuDefeated, bool zabuzaDefeated) =>
        hakuDefeated && zabuzaDefeated;
    public static byte NormalizeLegacyWaveFlags(bool hakuDefeated, bool zabuzaDefeated) =>
        EncounterComplete(hakuDefeated, zabuzaDefeated) ? (byte)3 : (byte)0;
}
