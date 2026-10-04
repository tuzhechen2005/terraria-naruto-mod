using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items.Jutsu;

namespace ShinobiPrototype.Common.Players;

// Hand seals (specs/装备与忍术系统.spec.md): hold the seal key and a seal forms every quarter second, up to six, while
// the player moves slowly and cannot attack; let go and the scroll of the highest slot reached (2, 4 or 6) goes off,
// for its chakra. A hit that gets through (no log, no clone to take it) breaks the seals; one a log takes does not.
public sealed class SealPlayer : ModPlayer
{
    // Filled by the seal slots during the equipment update; copied to `worn` once it is done, so the key (read before
    // equipment is updated) always sees a whole tick's worth.
    private readonly Item[] equipped = new Item[3];
    private readonly Item[] worn = new Item[3];
    private int heldTicks;

    public bool Weaving { get; private set; }
    public int Seals => SealRules.SealsAfter(heldTicks);
    // Ninjutsu power: a multiplier on seal jutsu damage from Naruto gear, reset every tick.
    public float NinjutsuPower { get; set; } = 1f;

    public override void ResetEffects()
    {
        equipped[0] = equipped[1] = equipped[2] = null;
        NinjutsuPower = 1f;
    }

    public void Equip(int seals, Item scroll)
    {
        int i = SealRules.SlotIndex(seals);
        if (i >= 0)
            equipped[i] = scroll;
    }

    public override void PostUpdateEquips()
    {
        for (int i = 0; i < 3; i++)
            worn[i] = equipped[i];
    }

    public SealScroll ScrollFor(int tier)
    {
        int i = SealRules.SlotIndex(tier);
        return i >= 0 ? worn[i]?.ModItem as SealScroll : null;
    }

    // The scroll that would go off if the key were let go now.
    public SealScroll Ready => ScrollFor(SealRules.Tier(Seals, worn[0] != null, worn[1] != null, worn[2] != null));

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        ModKeybind key = ShinobiKeybinds.Seal;
        if (key == null)
            return;
        if (key.JustPressed && CanWeave())
        {
            Weaving = true;
            heldTicks = 0;
        }
        else if (Weaving && !key.Current)
            Release();
    }

    private bool CanWeave() => !Player.dead && !Player.CCed && Player.itemAnimation == 0 &&
                               !Player.GetModPlayer<JutsuStatusPlayer>().SubstitutionSealed;

    public override void PostUpdate()
    {
        if (!Weaving)
            return;
        if (Player.dead || Player.CCed)
        {
            Cancel(null);
            return;
        }
        int before = Seals;
        heldTicks++;
        if (Seals > before)
        {
            SoundEngine.PlaySound(SoundID.MenuTick with { Pitch = -0.2f + 0.1f * Seals, Volume = 0.9f }, Player.Center);
            for (int i = 0; i < 6; i++)
                Dust.NewDustPerfect(Player.Center + new Vector2(Player.direction * 8f, -6f) + Main.rand.NextVector2Circular(6f, 6f),
                    DustID.BlueTorch, Main.rand.NextVector2Circular(1.5f, 1.5f), 0, default, 1.1f).noGravity = true;
        }
    }

    // Slow while forming seals.
    public override void PostUpdateRunSpeeds()
    {
        if (!Weaving)
            return;
        Player.maxRunSpeed *= SealRules.WeaveSpeed;
        Player.accRunSpeed *= SealRules.WeaveSpeed;
        Player.runAcceleration *= SealRules.WeaveSpeed;
    }

    public override bool CanUseItem(Item item) => !Weaving;

    // A hit that reached the player breaks the seals (a log or a clone taking it never gets here).
    public override void OnHurt(Player.HurtInfo info)
    {
        if (Weaving)
            Cancel("结印被打断了");
    }

    private void Release()
    {
        int seals = Seals;
        SealScroll scroll = Ready;
        Weaving = false;
        heldTicks = 0;
        if (scroll == null)
        {
            if (seals > 0)
                CombatText.NewText(Player.getRect(), Color.LightGray, seals < 2 ? "印还没结成" : "这个印位没有卷轴");
            return;
        }
        ChakraPlayer chakra = Player.GetModPlayer<ChakraPlayer>();
        if (!chakra.TrySpend(scroll.ChakraCost))
        {
            CombatText.NewText(Player.getRect(), new Color(120, 180, 255), "查克拉不足");
            return;
        }
        CombatText.NewText(Player.getRect(), new Color(255, 225, 150), scroll.Item.Name);
        scroll.Cast(Player);
    }

    private void Cancel(string why)
    {
        Weaving = false;
        heldTicks = 0;
        if (why != null)
            CombatText.NewText(Player.getRect(), new Color(200, 200, 200), why);
    }

    // A new character starts with the Clone Jutsu (specs: given at the start of tier one).
    public override System.Collections.Generic.IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
    {
        if (!mediumCoreDeath)
            yield return new Item(ModContent.ItemType<ScrollClone>());
    }
}
