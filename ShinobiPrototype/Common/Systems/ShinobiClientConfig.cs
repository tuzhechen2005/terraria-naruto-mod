using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace ShinobiPrototype.Common.Systems;

public sealed class ShinobiClientConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;

    public static ShinobiClientConfig Instance => ModContent.GetInstance<ShinobiClientConfig>();

    // Offsets move the chakra bar from its default spot under the vanilla life display.
    [Range(-20, 1600)]
    [DefaultValue(0)]
    public int ChakraBarOffsetX;

    [Range(-70, 900)]
    [DefaultValue(0)]
    public int ChakraBarOffsetY;

    [DefaultValue(true)]
    public bool ShowSubstitutionHints;

    // Percent of the designed sea-mist thickness around the Wave Country bridge; 0 turns it off, 200 doubles it.
    [Range(0, 200)]
    [Increment(5)]
    [DefaultValue(100)]
    [Slider]
    public int SeaFogStrength;
}
