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
}
