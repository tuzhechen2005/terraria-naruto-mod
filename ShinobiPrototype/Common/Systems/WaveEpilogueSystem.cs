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

    // A new fight silences the epilogue at once (user, 2026-09-29: re-summoning Zabuza within the three minutes of
    // Sadness and Sorrow kept the sad track playing over his entrance).
    private static bool BossPresent() =>
        NPC.AnyNPCs(ModContent.NPCType<Content.NPCs.ZabuzaBoss>()) || NPC.AnyNPCs(ModContent.NPCType<Content.NPCs.HakuBoss>());

    public override void PostUpdateEverything()
    {
        if (musicTicks > 0 && BossPresent())
            musicTicks = 0;
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
            float rate = WaveEpilogueRules.SnowRate(tick - WaveEpilogueRules.SnowFrom(hakuFirst),
                WaveEpilogueRules.Length(hakuFirst) - tick);
            if (rate > 0f)
                Snow(rate);
        }
        if (++tick >= WaveEpilogueRules.Length(hakuFirst))
        {
            tick = -1;
            // Then Kakashi turns up beside the player with the Chūnin Exam recommendation (M2 spec, section 2).
            Content.NPCs.Kakashi.ArriveAfterEpilogue(origin);
        }
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

    // Like vanilla's snowfall: flakes (dust 76) appear at random open-air spots anywhere in view and drift down,
    // so it is snowing the moment it starts. (The first version spawned one flake above the screen edge every other
    // tick with a dust that shrank away before it came into view, so no snow was seen.)
    private static void Snow(float rate)
    {
        int count = (int)rate + (Main.rand.NextFloat() < rate % 1f ? 1 : 0);
        for (int i = 0; i < count; i++)
        {
            Vector2 at = Main.screenPosition + new Vector2(Main.rand.NextFloat(-60f, Main.screenWidth + 60f),
                Main.rand.NextFloat(-40f, Main.screenHeight * 0.9f));
            Point tile = at.ToTileCoordinates();
            if (!WorldGen.InWorld(tile.X, tile.Y) || Main.tile[tile.X, tile.Y].HasTile)
                continue;
            Dust flake = Dust.NewDustPerfect(at, DustID.Snow,
                new Vector2(Main.WindForVisuals * 1.5f + Main.rand.NextFloat(-0.3f, 0.3f), Main.rand.NextFloat(1.5f, 3f)),
                0, default, Main.rand.NextFloat(0.9f, 1.3f));
            flake.noGravity = true;
        }
    }
}
