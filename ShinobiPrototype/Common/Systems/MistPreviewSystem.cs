using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Common.Systems;

// The one-time mist preview at the bridge: Zabuza on the water beyond the broken end, then Haku in her hunter mask,
// then both vanish. Pure client-side visuals and text, so nothing can be hit and nothing needs syncing beyond the
// start signal. The server (or single player) decides when it starts.
public sealed class MistPreviewSystem : ModSystem
{
    private static int tick = -1;

    public static float FogBoost => BridgeRules.PreviewFogBoost(tick);

    public static void Broadcast()
    {
        if (Main.netMode == NetmodeID.Server)
        {
            ModPacket packet = ModContent.GetInstance<ShinobiPrototype>().GetPacket();
            packet.Write((byte)ShinobiPrototype.Packet.PlayMistPreview);
            packet.Send();
            return;
        }
        Play();
    }

    public static void Play() => tick = 0;

    public override void OnWorldUnload() => tick = -1;

    public override void PostUpdateEverything()
    {
        if (tick < 0 || Main.dedServ || WaveBridgeWorld.Site is not BridgeSite site)
            return;

        switch (tick)
        {
            case BridgeRules.PreviewZabuzaAppear:
                Puff(ZabuzaBottom(site));
                break;
            case BridgeRules.PreviewZabuzaLine1:
                Say(ZabuzaBottom(site), "PreviewZabuza1", ZabuzaColor);
                break;
            case BridgeRules.PreviewZabuzaLine2:
                Say(ZabuzaBottom(site), "PreviewZabuza2", ZabuzaColor);
                break;
            case BridgeRules.PreviewHakuAppear:
                Puff(HakuBottom(site));
                break;
            case BridgeRules.PreviewHakuLine:
                Say(HakuBottom(site), "PreviewHaku", HakuColor);
                break;
            case BridgeRules.PreviewVanish:
                Puff(ZabuzaBottom(site));
                Puff(HakuBottom(site));
                Main.NewText(Language.GetTextValue("Mods.ShinobiPrototype.Dialogue.PreviewEnd"), 190, 200, 210);
                break;
        }
        if (++tick >= BridgeRules.PreviewLength)
            tick = -1;
    }

    private static readonly Color ZabuzaColor = new(160, 200, 230);
    private static readonly Color HakuColor = new(175, 240, 255);

    private static Vector2 ZabuzaBottom(BridgeSite site) =>
        new(site.X(BridgeRules.PreviewZabuzaOffset) * 16f + 8f, site.WaterY * 16f);

    private static Vector2 HakuBottom(BridgeSite site) =>
        new(site.X(BridgeRules.PreviewHakuOffset) * 16f + 8f, site.WaterY * 16f - 24f);

    private static void Say(Vector2 bottom, string key, Color color)
    {
        string text = Language.GetTextValue($"Mods.ShinobiPrototype.Dialogue.{key}");
        int colon = text.IndexOf('：');
        Rectangle area = new((int)bottom.X - 20, (int)bottom.Y - 90, 40, 90);
        CombatText.NewText(area, color, colon >= 0 ? text[(colon + 1)..] : text, dramatic: true);
        Main.NewText(text, color);
    }

    private static void Puff(Vector2 bottom)
    {
        for (int i = 0; i < 30; i++)
            Dust.NewDustPerfect(bottom + new Vector2(Main.rand.NextFloat(-24f, 24f), -Main.rand.NextFloat(0f, 90f)),
                DustID.Smoke, Main.rand.NextVector2Circular(1.5f, 1.5f), 100, new Color(210, 220, 230), 2f).noGravity = true;
    }

    // Called by the overlay before it lays the mist on top, so the pair shows through the fog.
    internal static void DrawFigures(SpriteBatch spriteBatch)
    {
        if (tick < 0 || WaveBridgeWorld.Site is not BridgeSite site)
            return;
        int facing = -site.Dir;
        float zabuza = Fade(BridgeRules.PreviewZabuzaAppear);
        float haku = Fade(BridgeRules.PreviewHakuAppear);
        if (zabuza > 0f)
            BossSprites.TryDraw(spriteBatch, "Zabuza", "Idle", BossSprites.Loop(8f, 4), 4, BossSprites.Zabuza,
                ZabuzaBottom(site), facing, Tint(ZabuzaBottom(site)) * zabuza, Main.screenPosition);
        if (haku > 0f)
            BossSprites.TryDraw(spriteBatch, "Haku", "Idle", BossSprites.Loop(8f, 4), 4, BossSprites.Haku,
                HakuBottom(site), facing, Tint(HakuBottom(site)) * haku, Main.screenPosition);
    }

    private static float Fade(int appear)
    {
        float fadeIn = MathHelper.Clamp((tick - appear) / 20f, 0f, 1f);
        float fadeOut = MathHelper.Clamp((BridgeRules.PreviewVanish + 10 - tick) / 10f, 0f, 1f);
        return fadeIn * fadeOut;
    }

    private static Color Tint(Vector2 bottom) =>
        BossSprites.Lit(Lighting.GetColor((int)(bottom.X / 16f), (int)(bottom.Y / 16f) - 2), 0.5f);
}
