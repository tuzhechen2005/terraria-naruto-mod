using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Systems;

// Wave Country music (user, 2026-09-29): Glued State around the bridge, Need to be Strong for Zabuza, Strong and
// Strike from his demon phase (and for Haku), Sadness and Sorrow for the epilogue.
// The tracks are the Naruto soundtrack, which the user supplies for local play: they live in Assets/Music/, are
// gitignored and must never be committed or published. When a file is missing the fight falls back to vanilla
// boss music and the scenes simply play no special track.
public static class WaveMusic
{
    private static int? gluedState, needToBeStrong, strongAndStrike, sadnessAndSorrow;

    public static int GluedState => gluedState ??= Slot("GluedState");
    public static int NeedToBeStrong => needToBeStrong ??= Slot("NeedToBeStrong");
    public static int StrongAndStrike => strongAndStrike ??= Slot("StrongAndStrike");
    public static int SadnessAndSorrow => sadnessAndSorrow ??= Slot("SadnessAndSorrow");

    public static int OrBossMusic(int slot) => slot >= 0 ? slot : MusicID.Boss1;

    private static int Slot(string name)
    {
        if (Main.dedServ)
            return -1;
        Mod mod = ModContent.GetInstance<ShinobiPrototype>();
        return mod.FileExists($"Assets/Music/{name}.mp3") ? MusicLoader.GetMusicSlot(mod, $"Assets/Music/{name}") : -1;
    }

    internal static void Reset()
    {
        gluedState = needToBeStrong = strongAndStrike = sadnessAndSorrow = null;
    }
}

// Glued State anywhere around the bridge (the sea-mist area), before and after the fight.
public sealed class WaveBridgeMusic : ModSceneEffect
{
    public override int Music => WaveMusic.GluedState;
    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

    public override bool IsSceneEffectActive(Player player) =>
        WaveMusic.GluedState >= 0 && WaveBridgeWorld.Site.HasValue &&
        WaveBridgeWorld.DistanceToBridgeTiles(player.Center) <= BridgeRules.FogReachTiles;
}

// Sadness and Sorrow once both have fallen, for the length of the track (unless the player wanders far off).
public sealed class WaveEpilogueMusic : ModSceneEffect
{
    public override int Music => WaveMusic.SadnessAndSorrow;
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

    public override bool IsSceneEffectActive(Player player) =>
        WaveMusic.SadnessAndSorrow >= 0 && WaveEpilogueSystem.MusicActive;

    public override void Unload() => WaveMusic.Reset();
}
