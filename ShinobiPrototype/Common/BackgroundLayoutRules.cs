using System;

namespace ShinobiPrototype.Common;

// Where a region background's close layer sits on screen (M2a spec, "地区背景"). tModLoader draws a mod style's close
// layer with its own formula (SurfaceBackgroundStylesLoader.DrawCloseBackground), which lands lower than vanilla's
// close layers: it leaves out the -150 push vanilla gives every front layer, and measures the camera from the top of
// the screen instead of 600 px above its middle. CloseOffset puts the layer back where vanilla would draw it, then
// lifts it so the art's ground line meets the horizon where vanilla's forest close layer has its own. Kept free of
// Terraria types so the rule tests can run it.
public static class BackgroundLayoutRules
{
    // ChooseCloseTexture's defaults; vanilla's close layers use the same numbers.
    public const float CloseA = 1800f;
    public const float CloseScale = 1.25f;
    // tModLoader doubles the close scale when it draws (Main.bgScale *= 2).
    public const float CloseDrawScale = CloseScale * 2f;
    // pushBGTopHack for the front layers in Main.DrawSurfaceBG: 30 in game, then -180.
    public const int FrontLayerPush = -150;
    // First fully opaque row of vanilla's forest close layer (Background_55, 1024 × 533).
    public const int VanillaCloseGroundRow = 342;
    // A row counts as ground once this share of its pixels is opaque.
    public const float GroundCoverage = 0.98f;

    // The first row from the top that is (almost) fully opaque: where the layer's ground begins. -1 if none is.
    public static int GroundRow(byte[] alpha, int width, int height)
    {
        int needed = (int)Math.Ceiling(width * GroundCoverage);
        for (int y = 0; y < height; y++)
        {
            int opaque = 0;
            for (int x = 0; x < width; x++)
            {
                if (alpha[y * width + x] > 128)
                    opaque++;
            }
            if (opaque >= needed)
                return y;
        }
        return -1;
    }

    // Added to ChooseCloseTexture's b. screenHeight / 2 is integer division, as in vanilla.
    public static float CloseOffset(int screenHeight, double worldSurface, int groundRow)
    {
        float vanillaPlacement = FrontLayerPush - (float)((screenHeight / 2 - 600) / (worldSurface * 16.0) * CloseA);
        float lift = groundRow < 0 ? 0f : (groundRow - VanillaCloseGroundRow) * CloseDrawScale;
        return vanillaPlacement - lift;
    }
}
