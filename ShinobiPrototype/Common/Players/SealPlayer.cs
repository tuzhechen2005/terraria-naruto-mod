using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items.Jutsu;

namespace ShinobiPrototype.Common.Players;

// Hand seals (specs/装备与忍术系统.spec.md). Three seal slots of the player's own (drawn by SealSlotsUI beside the
// inventory, saved with the character) hold one scroll each, of 2, 4 and 6 seals. A tap of a slot's key (Z, X, C)
// forms that many seals, a quarter second each, and casts its scroll for its chakra (user, 2026-10-03: holding one key
// and letting go at the right count was impossible mid-fight, and held a finger off WASD). While the seals form the
// player is slowed and cannot attack; a hit that gets through (no log, no clone to take it) breaks them, and the
// substitution key drops them for a blink.
public sealed class SealPlayer : ModPlayer
{
    // The three slots: 2, 4 and 6 seals.
    public Item[] Scrolls { get; } = { new(), new(), new() };

    private int heldTicks;
    private int target;
    private readonly int[] cooldowns = new int[3];

    private readonly int[] readyFlash = new int[3];

    public int CooldownOf(int slot) => cooldowns[slot];

    // Ticks left of the flash on the HUD when a slot comes off cooldown.
    public int ReadyFlash(int slot) => readyFlash[slot];

    public bool Weaving => target > 0;
    public int Seals => SealRules.SealsAfter(heldTicks);
    // The scroll being formed.
    public SealScroll Forming => target > 0 ? ScrollFor(target) : null;
    // Set each tick by a technique that holds the player to a creep and keeps their hands busy (Chidori gathering).
    public int ChargingTicks { get; set; }
    // Ninjutsu power: a multiplier on seal jutsu damage from Naruto gear, reset every tick.
    public float NinjutsuPower { get; set; } = 1f;

    public override void ResetEffects() => NinjutsuPower = 1f;

    public SealScroll ScrollFor(int seals)
    {
        int i = SealRules.SlotIndex(seals);
        return i >= 0 ? Scrolls[i]?.ModItem as SealScroll : null;
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (Weaving)
            return;
        foreach (int seals in new[] { 2, 4, 6 })
            if (ShinobiKeybinds.SealKey(seals)?.JustPressed == true)
            {
                Begin(seals);
                return;
            }
    }

    private void Begin(int seals)
    {
        if (Player.dead || Player.CCed || Player.itemAnimation > 0)
            return;
        if (Player.GetModPlayer<JutsuStatusPlayer>().SubstitutionSealed)
        {
            CombatText.NewText(Player.getRect(), new Color(170, 200, 255), "点穴：查克拉被封，结不了印");
            return;
        }
        SealScroll scroll = ScrollFor(seals);
        if (scroll == null)
        {
            CombatText.NewText(Player.getRect(), Color.LightGray, $"{seals} 印位没有卷轴");
            return;
        }
        if (cooldowns[SealRules.SlotIndex(seals)] > 0)
        {
            CombatText.NewText(Player.getRect(), Color.LightGray, $"{scroll.Item.Name}冷却中 {cooldowns[SealRules.SlotIndex(seals)] / 60f:0.0}s");
            return;
        }
        if (Player.GetModPlayer<ChakraPlayer>().Chakra < scroll.ChakraCost)
        {
            CombatText.NewText(Player.getRect(), new Color(120, 180, 255), "查克拉不足");
            return;
        }
        target = seals;
        heldTicks = 0;
    }

    public override void PostUpdate()
    {
        if (ChargingTicks > 0)
            ChargingTicks--;
        for (int i = 0; i < cooldowns.Length; i++)
        {
            if (readyFlash[i] > 0)
                readyFlash[i]--;
            if (cooldowns[i] > 0 && --cooldowns[i] == 0)
                readyFlash[i] = 20;
        }
        if (!Weaving)
            return;
        if (Player.dead || Player.CCed || Forming == null)
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
        if (Seals >= target)
            Cast();
    }

    // Slow while forming seals.
    public override void PostUpdateRunSpeeds()
    {
        if (!Weaving && ChargingTicks <= 0)
            return;
        float speed = Weaving ? SealRules.WeaveSpeed : SealRules.ChidoriGatherSpeed;
        Player.maxRunSpeed *= speed;
        Player.accRunSpeed *= speed;
        Player.runAcceleration *= speed;
    }

    public override bool CanUseItem(Item item) => !Weaving && ChargingTicks <= 0;

    // A hit that reached the player breaks the seals (a log or a clone taking it never gets here).
    public override void OnHurt(Player.HurtInfo info)
    {
        if (Weaving)
            Cancel("结印被打断了");
    }

    private void Cast()
    {
        SealScroll scroll = Forming;
        int slot = SealRules.SlotIndex(target);
        target = 0;
        heldTicks = 0;
        if (scroll == null)
            return;
        if (!Player.GetModPlayer<ChakraPlayer>().TrySpend(scroll.ChakraCost))
        {
            CombatText.NewText(Player.getRect(), new Color(120, 180, 255), "查克拉不足");
            return;
        }
        CombatText.NewText(Player.getRect(), new Color(255, 225, 150), scroll.Item.Name);
        if (slot >= 0)
            cooldowns[slot] = scroll.CooldownTicks;
        scroll.Cast(Player);
    }

    public void Cancel(string why)
    {
        if (!Weaving)
            return;
        target = 0;
        heldTicks = 0;
        if (why != null)
            CombatText.NewText(Player.getRect(), new Color(200, 200, 200), why);
    }

    public override void SaveData(TagCompound tag)
    {
        var list = new List<TagCompound>();
        foreach (Item scroll in Scrolls)
            list.Add(ItemIO.Save(scroll ?? new Item()));
        tag["sealScrolls"] = list;
    }

    public override void LoadData(TagCompound tag)
    {
        IList<TagCompound> list = tag.GetList<TagCompound>("sealScrolls");
        if (list.Count == 0)
            return;
        for (int i = 0; i < Scrolls.Length; i++)
            Scrolls[i] = i < list.Count ? ItemIO.Load(list[i]) : new Item();
    }

    // Every character starts with the Clone Jutsu in its 2-seal slot (specs: given at the start of tier one); a saved
    // character's own slots replace it in LoadData.
    public override void Initialize()
    {
        Scrolls[0] = new Item(ModContent.ItemType<ScrollClone>());
        Scrolls[1] = new Item();
        Scrolls[2] = new Item();
        target = heldTicks = 0;
    }
}
