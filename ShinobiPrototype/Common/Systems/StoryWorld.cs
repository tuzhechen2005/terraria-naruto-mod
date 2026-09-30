using System.IO;
using Terraria;
using Terraria.ID;
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
    // The mission chain (M1 spec, "剧情补充"): met Tazuna at his hut, heard him confess, and the lake ambush done.
    public static bool MetTazuna { get; set; }
    public static bool TazunaConfessed { get; set; }
    public static bool LakeDone { get; set; }
    // Zabuza has been summoned in this world: the mist teasers are over (StoryRules.TeasersAllowed).
    public static bool ZabuzaFought { get; set; }
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
        MetTazuna = false;
        TazunaConfessed = false;
        LakeDone = false;
        ZabuzaFought = false;
    }

    public override void OnWorldUnload()
    {
        DownedHaku = false;
        DownedZabuza = false;
        DownedArenaRival = false;
        DownedDemonBrothers = false;
        MetTazuna = false;
        TazunaConfessed = false;
        LakeDone = false;
        ZabuzaFought = false;
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
        if (MetTazuna)
            tag["metTazuna"] = true;
        if (TazunaConfessed)
            tag["tazunaConfessed"] = true;
        if (LakeDone)
            tag["lakeDone"] = true;
        if (ZabuzaFought)
            tag["zabuzaFought"] = true;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        byte wave = WaveDuoRules.NormalizeLegacyWaveFlags(tag.GetBool("downedHaku"),
            tag.GetBool("downedZabuza"));
        DownedHaku = (wave & 1) != 0;
        DownedZabuza = (wave & 2) != 0;
        DownedArenaRival = tag.GetBool("downedArenaRival");
        DownedDemonBrothers = tag.GetBool("downedDemonBrothers");
        MetTazuna = tag.GetBool("metTazuna");
        TazunaConfessed = tag.GetBool("tazunaConfessed");
        LakeDone = tag.GetBool("lakeDone");
        ZabuzaFought = tag.GetBool("zabuzaFought");
    }

    public override void NetSend(BinaryWriter writer)
    {
        byte flags = 0;
        if (DownedHaku) flags |= 1;
        if (DownedZabuza) flags |= 2;
        if (DownedArenaRival) flags |= 4;
        if (DownedDemonBrothers) flags |= 8;
        if (MetTazuna) flags |= 16;
        if (TazunaConfessed) flags |= 32;
        if (LakeDone) flags |= 64;
        if (ZabuzaFought) flags |= 128;
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
        MetTazuna = (flags & 16) != 0;
        TazunaConfessed = (flags & 32) != 0;
        LakeDone = (flags & 64) != 0;
        ZabuzaFought = (flags & 128) != 0;
    }

    // Talking to Tazuna happens on a client; the server records it and sends the world data back out.
    public static void RecordTazunaTalk(bool confessed)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            ModPacket packet = ModContent.GetInstance<ShinobiPrototype>().GetPacket();
            packet.Write((byte)ShinobiPrototype.Packet.TazunaTalk);
            packet.Write(confessed);
            packet.Send();
        }
        ApplyTazunaTalk(confessed);
    }

    internal static void ApplyTazunaTalk(bool confessed)
    {
        MetTazuna = true;
        TazunaConfessed |= confessed;
        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.WorldData);
    }
}
