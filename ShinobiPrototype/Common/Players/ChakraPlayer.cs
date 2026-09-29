using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Players;

public sealed class ChakraPlayer : ModPlayer
{
    public const int MaximumChakra = 100;
    public const int RasenganCost = 35;

    private float chakra = MaximumChakra;
    private int recoveryDelay;

    public int Chakra => (int)chakra;

    public override void Initialize()
    {
        chakra = MaximumChakra;
        recoveryDelay = 0;
    }

    public override void PostUpdate()
    {
        if (recoveryDelay > 0)
            recoveryDelay--;

        // 3 points per second during combat; faster when the player is safe.
        float regenerationPerTick = recoveryDelay > 0 ? 3f / 60f : 10f / 60f;
        chakra = MathHelper.Clamp(chakra + regenerationPerTick, 0f, MaximumChakra);
    }

    public bool TrySpend(int amount)
    {
        if (chakra < amount)
            return false;

        chakra -= amount;
        recoveryDelay = 300;
        return true;
    }

    public void Restore(int amount)
    {
        chakra = MathHelper.Clamp(chakra + amount, 0f, MaximumChakra);
        recoveryDelay = 300;
    }
}
