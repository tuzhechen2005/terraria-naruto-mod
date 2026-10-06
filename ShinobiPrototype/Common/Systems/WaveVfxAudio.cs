using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace ShinobiPrototype.Common.Systems;

// Existing Terraria sounds only. No local Naruto recordings are distributed.
// Caller fires each cue once on a phase transition, not once per drawing frame.
public static class WaveVfxAudio
{
    public enum Cue { WaterGather, DragonRelease, HeavySlash, NeedleRelease, MirrorBreak, ChakraCatch }

    public static void Play(Cue cue, Vector2 world)
    {
        if (Main.dedServ || Main.gameMenu)
            return;
        SoundStyle sound = cue switch
        {
            Cue.WaterGather => SoundID.Splash with { Volume = 0.4f, Pitch = -0.35f },
            Cue.DragonRelease => SoundID.Splash with { Volume = 0.85f, Pitch = -0.5f },
            Cue.HeavySlash => SoundID.Item1 with { Volume = 0.75f, Pitch = -0.35f },
            Cue.NeedleRelease => SoundID.Item28 with { Volume = 0.45f, Pitch = 0.25f },
            Cue.MirrorBreak => SoundID.Shatter with { Volume = 0.65f, Pitch = 0.15f },
            Cue.ChakraCatch => SoundID.Item7 with { Volume = 0.7f, Pitch = -0.4f },
            _ => SoundID.Splash
        };
        SoundEngine.PlaySound(sound with { MaxInstances = 4 }, world);
    }
}
