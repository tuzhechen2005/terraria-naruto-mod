using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ShinobiPrototype.Common.DamageClasses;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items.StyleCores;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Common.Players;

// The style cores on a character (specs/流派系统.spec.md): which core is worn this tick and its technique on the style
// key (cooldown, chakra), the effects each school keeps running (Sharingan foresight, the open Eight Gates, the
// Byakugan's Rotation), the school the character vowed to (立志) and whether the Third Hokage has asked for them.
public sealed class StyleCorePlayer : ModPlayer
{
    public StyleCore Worn { get; set; }
    public int Cooldown { get; private set; }
    public StyleSchool Vow { get; set; }
    // The Third Hokage sent word once the character held a core (M2 spec, 立志的剧情入口).
    public bool VowCalled { get; private set; }
    // Opened after Pain's assault on the Leaf (not in the game yet).
    public bool SecondSlot { get; set; }

    public StyleSchool School => Worn?.School ?? StyleSchool.None;

    // Sharingan.
    public int SharinganTier { get; set; }
    public int ForesightTicks { get; set; }
    private int foresightBonusTicks;
    private int seenTarget = -1;
    private int seenTicks;

    // Eight Gates.
    public int Gates { get; private set; }
    public bool GatesCore { get; set; }
    private int fatigueTicks;
    private float gateLifeDebt;

    // Byakugan.
    public bool Byakugan { get; set; }
    private int rotationTicks;

    public int SubstitutionWindowTicks => StyleCoreRules.SubstitutionWindowTicks(SharinganTier);

    public override void ResetEffects()
    {
        Worn = null;
        SharinganTier = 0;
        GatesCore = false;
        Byakugan = false;
    }

    public override void PostUpdateEquips()
    {
        if (!GatesCore)
            Gates = 0;
        if (Gates > 0)
        {
            Player.GetDamage(DamageClass.Generic) += StyleCoreRules.DamagePerGate * Gates;
            Player.moveSpeed += StyleCoreRules.SpeedPerGate * Gates;
            Player.maxRunSpeed *= 1f + StyleCoreRules.SpeedPerGate * Gates;
            Player.GetKnockback(DamageClass.Melee) += StyleCoreRules.KnockbackPerGate * Gates;
        }
        if (fatigueTicks > 0)
            Player.GetDamage(DamageClass.Generic) += StyleCoreRules.FatigueDamage;
        if (foresightBonusTicks > 0)
            Player.GetDamage(DamageClass.Generic) += StyleCoreRules.ForesightDamageBonus;
        if (Byakugan)
            Player.detectCreature = true;
    }

    public override void PostUpdate()
    {
        if (Cooldown > 0)
            Cooldown--;
        if (ForesightTicks > 0)
            ForesightTicks--;
        if (foresightBonusTicks > 0)
            foresightBonusTicks--;
        if (seenTicks > 0 && --seenTicks == 0)
            seenTarget = -1;
        if (fatigueTicks > 0)
            fatigueTicks--;

        if (Player.whoAmI != Main.myPlayer)
            return;
        if (StyleCoreRules.GatesForcedShut(Gates, Player.statLife, Player.statLifeMax2))
        {
            CloseGates();
            CombatText.NewText(Player.getRect(), new Color(120, 230, 120), "身体到极限了——门关上了");
        }
        if (Gates > 0 && Main.rand.NextBool(3))
            Dust.NewDust(Player.position, Player.width, Player.height, DustID.GreenTorch, 0f, -2f, 0, default, 1.2f);
        if (rotationTicks > 0)
            Rotation();
        CallForVow();
    }

    // The gates burn life while open.
    public override void UpdateBadLifeRegen()
    {
        if (Gates <= 0)
            return;
        if (Player.lifeRegen > 0)
            Player.lifeRegen = 0;
        Player.lifeRegenTime = 0;
        // lifeRegen counts half points of life a second.
        Player.lifeRegen -= 2 * StyleCoreRules.GateLifeLossPerSecond(Gates);
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (ShinobiKeybinds.StyleTechnique?.JustPressed != true)
            return;
        if (Worn is not StyleCore core)
        {
            Main.NewText("未装备流派核心。", 200, 200, 200);
            return;
        }
        ChakraPlayer chakra = Player.GetModPlayer<ChakraPlayer>();
        int cost = core.TechniqueCost(Player);
        if (!StyleCoreRules.CanUseTechnique(core.School, Cooldown, chakra.Chakra, cost))
        {
            Main.NewText(Cooldown > 0 ? "流派奥义冷却中。" : "查克拉不足。", 200, 200, 200);
            return;
        }
        if (cost > 0 && !chakra.TrySpend(cost))
            return;
        Cooldown = core.TechniqueCooldown;
        core.UseTechnique(Player);
    }

    // --- Sharingan: foresight takes the next hit with a substitution (SubstitutionPlayer asks here first).
    public void StartForesight(int ticks)
    {
        ForesightTicks = ticks;
        CombatText.NewText(Player.getRect(), new Color(230, 60, 60), "看破！");
    }

    public void ForesightTook(int attackerNpc)
    {
        ForesightTicks = 0;
        foresightBonusTicks = StyleCoreRules.ForesightBonusTicks;
        if (attackerNpc >= 0)
        {
            seenTarget = attackerNpc;
            seenTicks = StyleCoreRules.SeenTargetTicks;
        }
    }

    // Thrown kunai and senbon always crit the one the Sharingan saw through.
    public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
    {
        bool thrown = proj.type == ModContent.ProjectileType<KunaiThrown>() || proj.type == ModContent.ProjectileType<SenbonThrown>();
        if (thrown && target.whoAmI == seenTarget)
            modifiers.SetCrit();
    }

    // --- Eight Gates.
    public void PressGates()
    {
        int next = StyleCoreRules.NextGates(Gates);
        if (next == 0)
        {
            CloseGates();
            return;
        }
        Gates = next;
        string[] names = { "", "开门", "休门", "生门" };
        CombatText.NewText(Player.getRect(), new Color(120, 230, 120), $"八门遁甲·{names[Gates]}——开！", true);
        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item74, Player.Center);
    }

    private void CloseGates()
    {
        if (Gates == 0)
            return;
        Gates = 0;
        fatigueTicks = StyleCoreRules.FatigueTicks;
    }

    // --- Byakugan: melee hits seal points; the Rotation spins for a second.
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Byakugan && (hit.DamageType.CountsAsClass(DamageClass.Melee) || hit.DamageType.CountsAsClass<TaijutsuDamage>()))
            target.GetGlobalNPC<PointSealNPC>().Seal();
    }

    public void StartRotation()
    {
        rotationTicks = StyleCoreRules.RotationTicks;
        Player.SetImmuneTimeForAllTypes(StyleCoreRules.RotationTicks);
        CombatText.NewText(Player.getRect(), new Color(200, 220, 255), "八卦掌·回天！", true);
        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item60, Player.Center);
        float radius = StyleCoreRules.RotationRadiusTiles * 16f;
        foreach (NPC npc in Main.ActiveNPCs)
            if (!npc.friendly && !npc.dontTakeDamage && npc.Distance(Player.Center) < radius + npc.width / 2f)
                npc.SimpleStrikeNPC(StyleCoreRules.RotationDamage, npc.Center.X >= Player.Center.X ? 1 : -1, false, 9f);
    }

    private void Rotation()
    {
        rotationTicks--;
        float radius = StyleCoreRules.RotationRadiusTiles * 16f;
        foreach (Projectile projectile in Main.ActiveProjectiles)
            if (projectile.hostile && projectile.Distance(Player.Center) < radius)
                projectile.Kill();
        for (int i = 0; i < 4; i++)
            Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2CircularEdge(radius, radius), DustID.IceTorch,
                Vector2.Zero, 0, default, 1.3f).noGravity = true;
    }

    // --- The vow: the first time the character holds a core, Kakashi passes on that the Third Hokage wants them.
    private void CallForVow()
    {
        if (VowCalled || Vow != StyleSchool.None || Main.GameUpdateCount % 60 != 0 || !KonohaWorld.Site.HasValue)
            return;
        bool holds = Worn != null;
        for (int i = 0; i < 58 && !holds; i++)
            holds = Player.inventory[i].ModItem is StyleCore;
        if (!holds)
            return;
        VowCalled = true;
        Main.NewText("卡卡西：……哦？你拿到那东西了啊。三代大人叫你去火影楼一趟——别让老人家等太久哦。", 200, 210, 230);
    }

    public override void SaveData(TagCompound tag)
    {
        if (Vow != StyleSchool.None)
            tag["vow"] = (byte)Vow;
        if (VowCalled)
            tag["vowCalled"] = true;
        if (SecondSlot)
            tag["secondCoreSlot"] = true;
    }

    public override void LoadData(TagCompound tag)
    {
        Vow = (StyleSchool)tag.GetByte("vow");
        VowCalled = tag.GetBool("vowCalled");
        SecondSlot = tag.GetBool("secondCoreSlot");
    }
}

// Gentle Fist points on an enemy (Byakugan): defence down per stack, and bosses take more from every hit. Kept on
// the client that landed the hits, which is where that player's hits are worked out.
public sealed class PointSealNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    private int stacks;
    private int ticks;

    public void Seal()
    {
        stacks = StyleCoreRules.AddPoint(stacks);
        ticks = StyleCoreRules.PointTicks;
    }

    public override void ResetEffects(NPC npc)
    {
        if (ticks > 0 && --ticks == 0)
            stacks = 0;
    }

    public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
    {
        if (stacks <= 0)
            return;
        modifiers.Defense.Flat -= StyleCoreRules.PointDefensePerStack * stacks;
        if (npc.boss)
            modifiers.FinalDamage *= 1f + StyleCoreRules.PointBossDamagePerStack * stacks;
    }

    public override void DrawEffects(NPC npc, ref Color drawColor)
    {
        if (stacks > 0 && Main.rand.NextBool(12 - stacks * 2))
            Dust.NewDust(npc.position, npc.width, npc.height, DustID.IceTorch, 0f, -1f, 0, default, 0.9f);
    }
}
