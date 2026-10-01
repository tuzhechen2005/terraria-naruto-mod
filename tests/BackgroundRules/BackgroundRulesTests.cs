using static ShinobiPrototype.Common.BackgroundLayoutRules;

static void Check(bool condition, string name)
{
    if (!condition)
        throw new Exception(name);
    Console.WriteLine($"PASS {name}");
}

static byte[] Layer(int width, int height, int groundFrom, int bushFrom)
{
    byte[] alpha = new byte[width * height];
    for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
            alpha[y * width + x] = (byte)(y >= groundFrom || y >= bushFrom && x % 3 == 0 ? 255 : 0);
    return alpha;
}

Check(GroundRow(Layer(100, 50, 30, 20), 100, 50) == 30, "Ground starts at the first fully opaque row, not at the bushes above it");
Check(GroundRow(Layer(100, 50, 99, 20), 100, 50) == -1, "A layer with no solid ground has no ground row");
byte[] ragged = Layer(100, 50, 30, 99);
ragged[30 * 100 + 5] = 0;
Check(GroundRow(ragged, 100, 50) == 30, "A stray transparent pixel does not push the ground down");

// Vanilla's close layer: (int)(num4 * 1800 + 1750) + scAdj + push, num4 measured from 600 px above the screen middle.
// tModLoader's: (int)(ratio * a + b) + scAdj, ratio measured from the top of the screen (with screenOff / 2).
static double VanillaTop(float screenY, int screenHeight, double worldSurface)
{
    float screenOff = screenHeight - 600f;
    double num4 = (-(screenY + screenHeight / 2 - 600f) + screenOff / 2f) / (worldSurface * 16.0);
    return num4 * 1800.0 + 1750.0 + FrontLayerPush;
}
static double ModTop(float screenY, int screenHeight, double worldSurface, float b)
{
    float screenOff = screenHeight - 600f;
    return (-screenY + screenOff / 2f) / (worldSurface * 16.0) * CloseA + b;
}
bool matches = true;
foreach (int height in new[] { 768, 1080, 1440, 2160 })
    foreach (double surface in new[] { 300.0, 450.0, 600.0 })
        foreach (float screenY in new[] { 3000f, 4500f, 7000f })
            matches &= Math.Abs(ModTop(screenY, height, surface, 1750f + CloseOffset(height, surface, VanillaCloseGroundRow)) -
                VanillaTop(screenY, height, surface)) < 0.01;
Check(matches, "With vanilla's ground row, the corrected close layer lands exactly where vanilla draws its own");
Check(Math.Abs(CloseOffset(1080, 450, VanillaCloseGroundRow + 60) - CloseOffset(1080, 450, VanillaCloseGroundRow) + 60 * CloseDrawScale) < 0.01,
    "Art whose ground starts lower is lifted by the difference, at draw scale");
Check(CloseOffset(1080, 450, -1) == CloseOffset(1080, 450, VanillaCloseGroundRow), "Art without solid ground is not lifted");

// tModLoader lays a surface style's far and middle layers side by side every 1024 pixels (at its scale) whatever the
// texture's width: a narrower layer leaves a gap between copies (user, 2026-10-01: the Suna village had a slit of
// canyon showing through after its middle layer was cropped to 977). Every region layer must be 1024 wide.
string backgrounds = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "ShinobiPrototype", "Backgrounds");
foreach (string png in Directory.GetFiles(backgrounds, "*.png"))
{
    byte[] header = new byte[24];
    using (FileStream stream = File.OpenRead(png))
        stream.ReadExactly(header);
    int width = header[16] << 24 | header[17] << 16 | header[18] << 8 | header[19];
    Check(width == 1024, $"{Path.GetFileName(png)} is 1024 pixels wide");
}
