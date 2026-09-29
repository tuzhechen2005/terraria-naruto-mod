using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Systems;

// The sea mist around the Wave Country bridge (client-side visuals): works out how thick the mist is near the
// player and drives SeaMistOverlay, which draws it. The mist is the same grey-white as Zabuza's phase-two mist.
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
        overlay.Step();
    }
}
