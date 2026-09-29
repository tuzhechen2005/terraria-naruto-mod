using System.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ShinobiPrototype.Common;

namespace ShinobiPrototype.Common.Systems;

public sealed class StoryWorld : ModSystem
{
    public static bool DownedHaku { get; set; }
    public static bool DownedZabuza { get; set; }
    public static bool DownedArenaRival { get; set; }
    public static bool DownedDemonBrothers { get; set; }
    public static bool WaveComplete => ExamRules.WaveComplete(DownedHaku, DownedZabuza);

    public static void CompleteWave()
    {
        DownedHaku = true;
        DownedZabuza = true;
    }

    public override void OnWorldLoad()
    {
        DownedHaku = false;
        DownedZabuza = false;
        DownedArenaRival = false;
        DownedDemonBrothers = false;
    }

    public override void OnWorldUnload()
    {
        DownedHaku = false;
        DownedZabuza = false;
        DownedArenaRival = false;
        DownedDemonBrothers = false;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        if (DownedHaku)
            tag["downedHaku"] = true;
        if (DownedZabuza)
            tag["downedZabuza"] = true;
        if (DownedArenaRival)
            tag["downedArenaRival"] = true;
        if (DownedDemonBrothers)
            tag["downedDemonBrothers"] = true;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        byte wave = WaveDuoRules.NormalizeLegacyWaveFlags(tag.GetBool("downedHaku"),
            tag.GetBool("downedZabuza"));
        DownedHaku = (wave & 1) != 0;
        DownedZabuza = (wave & 2) != 0;
        DownedArenaRival = tag.GetBool("downedArenaRival");
        DownedDemonBrothers = tag.GetBool("downedDemonBrothers");
    }

    public override void NetSend(BinaryWriter writer)
    {
        byte flags = 0;
        if (DownedHaku) flags |= 1;
        if (DownedZabuza) flags |= 2;
        if (DownedArenaRival) flags |= 4;
        if (DownedDemonBrothers) flags |= 8;
        writer.Write(flags);
    }

    public override void NetReceive(BinaryReader reader)
    {
        byte flags = reader.ReadByte();
        byte wave = WaveDuoRules.NormalizeLegacyWaveFlags((flags & 1) != 0,
            (flags & 2) != 0);
        DownedHaku = (wave & 1) != 0;
        DownedZabuza = (wave & 2) != 0;
        DownedArenaRival = (flags & 4) != 0;
        DownedDemonBrothers = (flags & 8) != 0;
    }
}
