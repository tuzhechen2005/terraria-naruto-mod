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
    public static bool DownedDemonBrothers { get; set; }
    // The mission chain (M1 spec, "剧情补充"): met Tazuna at his hut, heard him confess, and the lake ambush done.
    public static bool MetTazuna { get; set; }
    public static bool TazunaConfessed { get; set; }
    public static bool LakeDone { get; set; }
    // Zabuza has been summoned in this world: the mist teasers are over (StoryRules.TeasersAllowed).
    public static bool ZabuzaFought { get; set; }
    // The Chūnin Exams (specs/M2_中忍考试篇.spec.md): boss kills are the world's; each character's own steps are in
    // ChuninExamPlayer.
    public static bool DownedDosu { get; set; }
    public static bool DownedGaara { get; set; }
    public static bool DownedNeji { get; set; }
    public static bool OrochimaruMet { get; set; }
    public static bool WaveComplete => DownedHaku && DownedZabuza;

    public static void CompleteWave()
    {
        DownedHaku = true;
        DownedZabuza = true;
    }

    public override void OnWorldLoad()
    {
        DownedHaku = false;
        DownedZabuza = false;
        DownedDemonBrothers = false;
        MetTazuna = false;
        TazunaConfessed = false;
        LakeDone = false;
        ZabuzaFought = false;
        DownedDosu = false;
        DownedGaara = false;
        DownedNeji = false;
        OrochimaruMet = false;
    }

    public override void OnWorldUnload()
    {
        DownedHaku = false;
        DownedZabuza = false;
        DownedDemonBrothers = false;
        MetTazuna = false;
        TazunaConfessed = false;
        LakeDone = false;
        ZabuzaFought = false;
        DownedDosu = false;
        DownedGaara = false;
        DownedNeji = false;
        OrochimaruMet = false;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        if (DownedHaku)
            tag["downedHaku"] = true;
        if (DownedZabuza)
            tag["downedZabuza"] = true;
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
        if (DownedDosu)
            tag["downedDosu"] = true;
        if (DownedGaara)
            tag["downedGaara"] = true;
        if (DownedNeji)
            tag["downedNeji"] = true;
        if (OrochimaruMet)
            tag["orochimaruMet"] = true;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        byte wave = WaveDuoRules.NormalizeLegacyWaveFlags(tag.GetBool("downedHaku"),
            tag.GetBool("downedZabuza"));
        DownedHaku = (wave & 1) != 0;
        DownedZabuza = (wave & 2) != 0;
        DownedDemonBrothers = tag.GetBool("downedDemonBrothers");
        MetTazuna = tag.GetBool("metTazuna");
        TazunaConfessed = tag.GetBool("tazunaConfessed");
        LakeDone = tag.GetBool("lakeDone");
        ZabuzaFought = tag.GetBool("zabuzaFought");
        DownedDosu = tag.GetBool("downedDosu");
        DownedGaara = tag.GetBool("downedGaara");
        DownedNeji = tag.GetBool("downedNeji");
        OrochimaruMet = tag.GetBool("orochimaruMet");
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write(new BitsByte(DownedHaku, DownedZabuza, DownedDemonBrothers, MetTazuna, TazunaConfessed, LakeDone,
            ZabuzaFought));
        writer.Write(new BitsByte(DownedDosu, DownedGaara, DownedNeji, OrochimaruMet));
    }

    public override void NetReceive(BinaryReader reader)
    {
        BitsByte wave = reader.ReadByte();
        byte normalized = WaveDuoRules.NormalizeLegacyWaveFlags(wave[0], wave[1]);
        DownedHaku = (normalized & 1) != 0;
        DownedZabuza = (normalized & 2) != 0;
        DownedDemonBrothers = wave[2];
        MetTazuna = wave[3];
        TazunaConfessed = wave[4];
        LakeDone = wave[5];
        ZabuzaFought = wave[6];
        BitsByte exam = reader.ReadByte();
        DownedDosu = exam[0];
        DownedGaara = exam[1];
        DownedNeji = exam[2];
        OrochimaruMet = exam[3];
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
