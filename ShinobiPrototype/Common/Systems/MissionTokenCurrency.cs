using Terraria.GameContent.UI;
using Terraria.ModLoader;
using ShinobiPrototype.Content.Items.Tier1;

namespace ShinobiPrototype.Common.Systems;

// Mission tokens as a shop currency, like vanilla's Defender Medals: Iruka's exchange prices in them.
public sealed class MissionTokenCurrency : ModSystem
{
    public static int Id { get; private set; } = -1;

    // After the mod's content has its types and before the shops are built.
    public override void OnModLoad() =>
        Id = CustomCurrencyManager.RegisterCurrency(new CustomCurrencySingleCoin(ModContent.ItemType<MissionToken>(), 9999L));
}
