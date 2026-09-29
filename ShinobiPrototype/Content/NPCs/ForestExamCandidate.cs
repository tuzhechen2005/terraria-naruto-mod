using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;

namespace ShinobiPrototype.Content.NPCs;

public abstract class ForestExamCandidate : ModNPC
{
    public override string Texture => "ShinobiPrototype/Content/NPCs/ForestExamCandidate";

    protected abstract bool Surface { get; }
    protected abstract int ScrollType { get; }

    public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 1;

    public override void SetDefaults()
    {
        NPC.width = 28;
        NPC.height = 46;
        NPC.damage = 19;
        NPC.defense = 3;
        NPC.lifeMax = 110;
        NPC.knockBackResist = 0.4f;
        NPC.aiStyle = -1;
        NPC.value = 45f;
        NPC.color = Surface ? new Color(180, 225, 170) : new Color(155, 170, 210);
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        Player player = spawnInfo.Player;
        bool correctDepth = Surface ? player.ZoneOverworldHeight :
            player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight;
        return ExamRules.CanSpawnTrialEnemy(player.GetModPlayer<StoryPlayer>().ExamStage,
            StoryWorld.DownedArenaRival, player.ZoneJungle, correctDepth) ? 0.65f : 0f;
    }

    public override void AI()
    {
        NPC.TargetClosest();
        Player target = Main.player[NPC.target];
        if (!target.active || target.dead)
            return;

        float distance = target.Center.X - NPC.Center.X;
        NPC.direction = distance >= 0f ? 1 : -1;
        NPC.spriteDirection = NPC.direction;
        float speed = Surface ? 2.2f : 2.7f;
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * speed, 0.07f);
        if (NPC.collideX && NPC.velocity.Y == 0f)
            NPC.velocity.Y = -7f;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) =>
        npcLoot.Add(ItemDropRule.Common(ScrollType));
}

public sealed class ForestCanopyCandidate : ForestExamCandidate
{
    protected override bool Surface => true;
    protected override int ScrollType => ModContent.ItemType<HeavenScroll>();
}

public sealed class ForestBurrowCandidate : ForestExamCandidate
{
    protected override bool Surface => false;
    protected override int ScrollType => ModContent.ItemType<EarthScroll>();
}
