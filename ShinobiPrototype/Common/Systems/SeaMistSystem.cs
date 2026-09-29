using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Systems;

// The sea mist around the Wave Country bridge (client-side visuals): works out how thick the mist is near the
// player, drives SeaMistOverlay (the grey-white veil reused from Zabuza's phase-two mist) and releases slow, faint
// drifting puffs of mist across the view. Puffs take the world light, so they are dim at night.
public sealed class SeaMistSystem : ModSystem
{
    private const string OverlayKey = "ShinobiPrototype:SeaMist";
    private static SeaMistOverlay overlay;

    // Mist density 0..1 near the player right now; the preview thickens it.
    public static float Density { get; private set; }

    public override void Load()
    {
        if (Main.dedServ)
            return;
        overlay = new SeaMistOverlay();
        Overlays.Scene[OverlayKey] = overlay;
    }

    public override void Unload() => overlay = null;

    public override void OnWorldUnload() => Density = 0f;

    public override void PostUpdateEverything()
    {
        if (Main.dedServ || Main.LocalPlayer is not { active: true } player)
            return;

        float target = 0f;
        if (WaveBridgeWorld.MistActive)
            target = BridgeRules.SeaMist(WaveBridgeWorld.DistanceToBridgeTiles(player.Center),
                !Main.dayTime || Main.raining, ShinobiClientConfig.Instance.SeaFogStrength / 100f);
        target = System.Math.Max(target, MistPreviewSystem.MistBoost);
        Density = MathHelper.Lerp(Density, target, 0.05f);
        if (overlay == null || Density <= 0.01f)
            return;
        if (overlay.Mode != OverlayMode.Active)
            Overlays.Scene.Activate(OverlayKey, player.Center);
        if (Main.GameUpdateCount % 3 == 0 && Main.rand.NextFloat() < Density * 1.5f)
            DriftPuff();
    }

    // A slow, faint round puff somewhere in view (lighter than the first version's, user 2026-09-29).
    private static void DriftPuff()
    {
        Vector2 at = Main.screenPosition + new Vector2(Main.rand.NextFloat(Main.screenWidth), Main.rand.NextFloat(Main.screenHeight));
        Dust puff = Dust.NewDustPerfect(at, Terraria.ID.DustID.Smoke, new Vector2(Main.rand.NextFloat(0.2f, 0.6f), 0f),
            215, new Color(200, 214, 224), Main.rand.NextFloat(2f, 3.2f));
        puff.noGravity = true;
    }
}
