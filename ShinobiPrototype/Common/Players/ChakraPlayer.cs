using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ShinobiPrototype.Common.Players;

public sealed class ChakraPlayer : ModPlayer
{
    public const int RasenganCost = 35;

    private float chakra = ChakraRules.BaseMaxChakra;
    private int recoveryDelay;
    private int hitWindowTicks;
    private int hitRestoredThisWindow;

    public int Crystals { get; private set; }
    public int MaxChakra => ChakraRules.MaxChakra(Crystals);
    public int Chakra => (int)chakra;

    public override void Initialize()
    {
        Crystals = 0;
        chakra = MaxChakra;
        recoveryDelay = 0;
        hitWindowTicks = 0;
        hitRestoredThisWindow = 0;
    }

    public override void PostUpdate()
    {
        if (recoveryDelay > 0)
            recoveryDelay--;
        if (hitWindowTicks > 0 && --hitWindowTicks == 0)
            hitRestoredThisWindow = 0;

        chakra = System.Math.Clamp(chakra + ChakraRules.RegenPerTick(recoveryDelay), 0f, MaxChakra);
    }

    public bool TrySpend(int amount)
    {
        if (chakra < amount)
            return false;

        chakra -= amount;
        recoveryDelay = ChakraRules.RegenDelayTicks;
        return true;
    }

    public void Restore(int amount) => chakra = System.Math.Clamp(chakra + amount, 0f, MaxChakra);

    public bool TryUseCrystal()
    {
        if (!ChakraRules.CanUseCrystal(Crystals))
            return false;

        Crystals++;
        Restore(ChakraRules.CrystalBonus);
        return true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Player.whoAmI != Main.myPlayer || target.friendly || target.immortal || target.lifeMax <= 5 ||
            NPCID.Sets.CountsAsCritter[target.type])
            return;

        int gain = ChakraRules.HitRegen(hitRestoredThisWindow);
        if (gain <= 0)
            return;

        if (hitRestoredThisWindow == 0)
            hitWindowTicks = ChakraRules.HitRegenWindowTicks;
        hitRestoredThisWindow += gain;
        Restore(gain);
    }

    public override void SaveData(TagCompound tag)
    {
        if (Crystals > 0)
            tag["chakraCrystals"] = Crystals;
    }

    public override void LoadData(TagCompound tag)
    {
        Crystals = System.Math.Clamp(tag.GetInt("chakraCrystals"), 0, ChakraRules.MaxCrystals);
        chakra = MaxChakra;
    }
}
