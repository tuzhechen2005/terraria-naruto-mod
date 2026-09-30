using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Systems;

// Wave Country music (user, 2026-09-29): Glued State around the bridge, the Sadness and Sorrow file for Zabuza's
// entrance, Strong and Strike from his demon phase (and for Haku), the Need to be Strong file for the epilogue.
// (The user first described these the other way round, then asked for the two to be swapped after hearing them.)
// The tracks are the Naruto soundtrack, which the user supplies for local play: they live in Assets/Music/, are
// gitignored and must never be committed or published. When a file is missing the fight falls back to vanilla
// boss music and the scenes simply play no special track.
public static class WaveMusic
{
    private static int? gluedState, needToBeStrong, strongAndStrike, sadnessAndSorrow;

    public static int GluedState => gluedState ??= Slot("GluedState");
    // Named by role: the entrance plays the SadnessAndSorrow file and the epilogue the NeedToBeStrong file.
    public static int ZabuzaEntrance => needToBeStrong ??= Slot("SadnessAndSorrow");
    public static int StrongAndStrike => strongAndStrike ??= Slot("StrongAndStrike");
    public static int Epilogue => sadnessAndSorrow ??= Slot("NeedToBeStrong");

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

// The epilogue track once both have fallen, for its length (unless the player wanders off or a new fight starts).
public sealed class WaveEpilogueMusic : ModSceneEffect
{
    public override int Music => WaveMusic.Epilogue;
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

    public override bool IsSceneEffectActive(Player player) =>
        WaveMusic.Epilogue >= 0 && WaveEpilogueSystem.MusicActive;

    public override void Unload() => WaveMusic.Reset();
}
