using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;

namespace ShinobiPrototype.Content.NPCs;

// Kakashi, the Leaf mentor: the only Naruto town NPC that turns up without a house (see KakashiSpawnSystem).
// His opening line carries the current objective; the buttons give tips and a substitution drill.
[AutoloadHead]
public sealed class Kakashi : ModNPC
{
    // Sheet layout, one frame per row, facing left like vanilla town NPCs.
    private const int IdleFrame = 0;
    private const int WalkFirst = 1;
    private const int WalkFrames = 6;
    private const int JumpFrame = 7;
    private const int SitFrame = 8;
    private const int ThrowFirst = 9;
    private const int ThrowFrames = 3;
    private const int FrameCount = 12;

    // Vanilla town AI states used for animation.
    private const float SittingState = 5f;
    private const float ThrowingState = 10f;

    private static int nextTip;

    // Substitution drill (local client only): the player being drilled, whether a kunai is being aimed,
    // and how long the release pose still shows.
    public int DrillTarget { get; set; } = -1;
    public bool DrillAiming { get; set; }
    public int DrillReleaseTicks { get; set; }

    public Vector2 DrillHand => NPC.Center + new Vector2(NPC.direction * 12f, -6f);

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = FrameCount;
        NPCID.Sets.DangerDetectRange[Type] = 500;
        NPCID.Sets.AttackType[Type] = 0;
        NPCID.Sets.AttackTime[Type] = 24;
        NPCID.Sets.AttackAverageChance[Type] = 20;
        NPCID.Sets.HatOffsetY[Type] = 4;
    }

    public override void SetDefaults()
    {
        NPC.townNPC = true;
        NPC.friendly = true;
        NPC.width = 18;
        NPC.height = 40;
        NPC.aiStyle = NPCAIStyleID.Passive;
        NPC.damage = 10;
        NPC.defense = 15;
        NPC.lifeMax = 250;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.5f;
        // A touch larger than vanilla town NPCs, so the mentor stands out; drawing stays anchored at the feet.
        NPC.scale = 1.1f;
    }

    public override bool CanTownNPCSpawn(int numTownNPCs) => true;

    public override List<string> SetNPCNameList() => new() { this.GetLocalizedValue("GivenName") };

    public override string GetChat()
    {
        string[] greetings =
        {
            "哟。",
            "……（合上手里的橙色小书）哟。",
            "抱歉来晚了，我在人生的道路上迷路了。",
        };
        Player player = Main.LocalPlayer;
        string objective = player.GetModPlayer<StoryPlayer>().CurrentObjective();

        // Characters made before the handbook existed (or who lost it) get one from him.
        int handbook = ModContent.ItemType<NinjaHandbook>();
        if (!player.HasItem(handbook))
        {
            player.QuickSpawnItem(NPC.GetSource_FromThis(), handbook);
            return "哟。你的忍者手册呢？……拿着，别再弄丢了。任务、忍术、查克拉的事都记在里面。\n\n" + objective;
        }

        // Worlds made before the bridge existed: hand over Tazuna's blueprint (again, if it was lost).
        int blueprint = ModContent.ItemType<BridgeBlueprint>();
        if (!WaveBridgeWorld.Site.HasValue && !player.HasItem(blueprint))
        {
            player.QuickSpawnItem(NPC.GetSource_FromThis(), blueprint);
            return "哟。造桥的达兹纳托我把这个交给你——他的施工图。这片海边还没有他的桥。" +
                   "到海滩上面朝大海用一次，看看轮廓；没问题的话原地再用一次，桥就立起来了。\n\n" + objective;
        }

        return $"{Main.rand.Next(greetings)}\n\n{objective}";
    }

    public override void SetChatButtons(ref string button, ref string button2)
    {
        button = "指点";
        button2 = "练习替身术";
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
        if (firstButton)
        {
            string[] tips = Tips();
            Main.npcChatText = tips[nextTip];
            nextTip = (nextTip + 1) % tips.Length;
            return;
        }

        StartPractice();
    }

    private static string[] Tips()
    {
        string key = ShinobiKeybinds.SubstitutionKeyName();
        return new[]
        {
            $"替身术：在挨打的前一刻按【{key}】，留下一截木头，人已经在别处了。耗 20 查克拉，之后要等 4 秒才能再用。按早了就白费——想练的话，点“练习替身术”。",
            "查克拉会自己慢慢恢复，刚用完术的几秒会慢一些。打中敌人也能回一点，不管你用刀、弓、魔法还是召唤物。",
            "蘑菇配太阳花，在工作台能做兵粮丸，一口回 40 查克拉。应急用的，别想着连着吃。",
            "地下洞穴里有发蓝光的查克拉结晶，就像生命水晶那样，用了能让查克拉上限变高。下矿的时候留意一下。",
            "雾隐的人在离村子远的地方活动，海边尤其多。拿到他们的标记，就能追到再不斩。",
            "我说过的这些，忍者手册里都记着，随时翻。",
        };
    }

    private void StartPractice()
    {
        Player player = Main.LocalPlayer;
        player.SetTalkNPC(-1);
        Main.npcChatText = "";
        CombatText.NewText(NPC.getRect(), Color.White, "看好了——");
        player.GetModPlayer<SubstitutionDrillPlayer>().Start(NPC.whoAmI);
    }

    // During a drill he stops wandering and faces the player; gravity and collision still run outside the AI.
    public override bool PreAI()
    {
        if (DrillReleaseTicks > 0)
            DrillReleaseTicks--;
        if (DrillTarget < 0 || !Main.player[DrillTarget].active)
            return true;

        NPC.velocity.X *= 0.8f;
        NPC.direction = Main.player[DrillTarget].Center.X >= NPC.Center.X ? 1 : -1;
        return false;
    }

    public override void TownNPCAttackStrength(ref int damage, ref float knockback)
    {
        damage = 14;
        knockback = 3f;
    }

    public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
    {
        cooldown = 20;
        randExtraCooldown = 10;
    }

    public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
    {
        projType = ProjectileID.ThrowingKnife;
        attackDelay = 8;
    }

    public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
    {
        multiplier = 11f;
        gravityCorrection = 0f;
        randomOffset = 0.5f;
    }

    public override void FindFrame(int frameHeight)
    {
        // Overriding FindFrame skips vanilla's town-NPC framing, which is also what turns the sprite to face
        // the way he walks; without this he always faces left and walks backwards when heading right.
        NPC.spriteDirection = NPC.direction;

        int frame;
        if (DrillAiming)
            frame = ThrowFirst;
        else if (DrillReleaseTicks > 0)
            frame = ThrowFirst + (DrillReleaseTicks > 8 ? 1 : 2);
        else if (NPC.ai[0] == SittingState)
            frame = SitFrame;
        else if (NPC.ai[0] == ThrowingState)
        {
            float progress = 1f - NPC.ai[1] / Math.Max(1, NPCID.Sets.AttackTime[Type]);
            frame = ThrowFirst + Math.Clamp((int)(progress * ThrowFrames), 0, ThrowFrames - 1);
        }
        else if (NPC.velocity.Y != 0f)
            frame = JumpFrame;
        else if (Math.Abs(NPC.velocity.X) > 0.1f)
        {
            NPC.frameCounter += Math.Abs(NPC.velocity.X);
            frame = WalkFirst + (int)(NPC.frameCounter / 8.0) % WalkFrames;
        }
        else
        {
            NPC.frameCounter = 0;
            frame = IdleFrame;
        }

        NPC.frame.Y = frame * frameHeight;
    }
}
