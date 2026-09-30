using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Common.Systems;

// The epilogue once both Zabuza and Haku have fallen (user, 2026-09-29): Sadness and Sorrow plays, the one who fell
// last speaks their final lines (Zabuza drags himself to Haku's side if she fell first), snow begins to fall, and
// both bodies fade. Started by the server (or single player); lines, snow and music are client-side.
public sealed class WaveEpilogueSystem : ModSystem
{
    private static int tick = -1;
    private static bool hakuFirst;
    private static int musicTicks;
    private static Vector2 origin;

    public static bool Playing => tick >= 0;
    public static bool HakuFellFirst => hakuFirst;
    public static int Tick => tick;
    public static bool MusicActive => musicTicks > 0;
    public static bool Fading => Playing && tick >= WaveEpilogueRules.Length(hakuFirst) - WaveEpilogueRules.FadeTicks;

    public static void Begin(bool hakuFellFirst, Vector2 where)
    {
        if (Main.netMode == NetmodeID.Server)
        {
            ModPacket packet = ModContent.GetInstance<ShinobiPrototype>().GetPacket();
            packet.Write((byte)ShinobiPrototype.Packet.WaveEpilogue);
            packet.Write(hakuFellFirst);
            packet.WriteVector2(where);
            packet.Send();
        }
        Start(hakuFellFirst, where);
    }

    internal static void Start(bool hakuFellFirst, Vector2 where)
    {
        tick = 0;
        hakuFirst = hakuFellFirst;
        origin = where;
        musicTicks = Main.dedServ ? 0 : WaveEpilogueRules.MusicTicks;
    }

    public override void OnWorldUnload()
    {
        tick = -1;
        musicTicks = 0;
    }

    public override void PostUpdateEverything()
    {
        if (musicTicks > 0)
        {
            musicTicks--;
            if (Main.LocalPlayer is { active: true } player &&
                player.Distance(origin) > WaveEpilogueRules.MusicLeaveTiles * 16f)
                musicTicks = 0;
        }
        if (tick < 0)
            return;

        if (!Main.dedServ)
        {
            foreach (WaveEpilogueRules.Beat beat in WaveEpilogueRules.Beats(hakuFirst))
                if (beat.Tick == tick)
                    Speak(beat.Key);
            if (tick >= WaveEpilogueRules.SnowFrom(hakuFirst))
                Snow();
        }
        if (++tick >= WaveEpilogueRules.Length(hakuFirst))
            tick = -1;
    }

    // Lines with a speaker ("Name：...") float over that body; narration goes to chat only.
    private static void Speak(string key)
    {
        string text = Language.GetTextValue($"Mods.ShinobiPrototype.Dialogue.{key}");
        int colon = text.IndexOf('：');
        bool zabuza = key.StartsWith("EpilogueZabuza");
        bool haku = key.StartsWith("EpilogueHaku");
        Color color = zabuza ? new Color(160, 200, 230) : haku ? new Color(175, 240, 255) : new Color(200, 205, 215);
        Main.NewText(text, color);
        if ((zabuza || haku) && WaveCorpse.Find(zabuza ? WaveCorpse.Zabuza : WaveCorpse.Haku) is Projectile body)
            CombatText.NewText(new Rectangle((int)body.Center.X - 20, (int)body.Center.Y - 70, 40, 40), color,
                colon >= 0 ? text[(colon + 1)..] : text, dramatic: true);
    }

    private static void Snow()
    {
        if (Main.GameUpdateCount % 2 != 0)
            return;
        Vector2 at = Main.screenPosition + new Vector2(Main.rand.NextFloat(-100f, Main.screenWidth + 100f), -20f);
        Dust flake = Dust.NewDustPerfect(at, DustID.Snow, new Vector2(Main.rand.NextFloat(-0.4f, 0.4f), Main.rand.NextFloat(1f, 2f)),
            0, default, Main.rand.NextFloat(0.8f, 1.3f));
        flake.noGravity = true;
        flake.fadeIn = 1.2f;
    }
}
