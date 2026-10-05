using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;

namespace ShinobiPrototype.Content.NPCs;

// "The boy in the forest" (M1 spec ④): Haku without his mask, gathering herbs in the forest near the bridge by day,
// before the fight. He stands to talk when the player comes close, talks about precious people, then walks off and
// is gone. A character who met him hears Haku recall it in the boss fight. Frames: Idle, Walk x6, Crouch x2, Talk.
public sealed class HakuForest : ModNPC
{
    private const int Gather = 0;
    private const int Talk = 1;
    private const int Leave = 2;
    private const float NoticeTiles = 10f;
    private const float WalkAwayTiles = 30f;
    private const int LeaveTicks = 180;
    private const float SpawnChanceValue = 0.012f;
    private const float NearBridgeTiles = 250f;

    private const int LineCount = 4;   // HakuForest.Line1..4, then HakuForest.Parting
    private static string Line(int index) => Loc.Get($"HakuForest.Line{index + 1}");

    private int line;
    private bool talked;

    private ref float State => ref NPC.ai[0];
    private ref float Timer => ref NPC.ai[1];

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 10;
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers { Hide = true });
    }

    public override void SetDefaults()
    {
        NPC.width = 18;
        NPC.height = 40;
        NPC.aiStyle = -1;
        NPC.friendly = true;
        NPC.dontTakeDamage = true;
        NPC.lifeMax = 100;
        NPC.knockBackResist = 0f;
        NPC.npcSlots = 0f;
        NPC.scale = NpcSheet.HiresScale;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        Player player = spawnInfo.Player;
        bool nearBridge = WaveBridgeWorld.DistanceToBridgeTiles(player.Center) < NearBridgeTiles;
        return Main.dayTime && !Main.raining && player.ZoneForest && player.ZoneOverworldHeight && nearBridge &&
               !StoryWorld.WaveComplete && !NPC.AnyNPCs(Type) && !NPC.AnyNPCs(ModContent.NPCType<HakuBoss>())
            ? SpawnChanceValue
            : 0f;
    }

    public override bool CanChat() => State != Leave;

    public override bool CheckActive() => State == Leave && Timer > LeaveTicks;

    public override string GetChat()
    {
        talked = true;
        Main.LocalPlayer.GetModPlayer<MistEncounterPlayer>().MeetForestBoy();
        return Line(Math.Min(line, LineCount - 1));
    }

    public override void SetChatButtons(ref string button, ref string button2) =>
        button = Loc.Get(line < LineCount - 1 ? "HakuForest.Continue" : "HakuForest.Farewell");

    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
        if (!firstButton)
            return;
        if (++line < LineCount)
        {
            Main.npcChatText = Line(line);
            return;
        }
        Main.npcChatText = Loc.Get("HakuForest.Parting");
        State = Leave;
        Timer = 0f;
        NPC.netUpdate = true;
    }

    public override void AI()
    {
        Timer++;
        Player nearest = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)];
        float distance = nearest.Distance(NPC.Center) / 16f;
        switch ((int)State)
        {
            case Gather:
                NPC.velocity.X = 0f;
                if (distance < NoticeTiles)
                    State = Talk;
                break;
            case Talk:
                NPC.velocity.X = 0f;
                NPC.direction = nearest.Center.X >= NPC.Center.X ? 1 : -1;
                if (talked && distance > WalkAwayTiles)
                {
                    State = Leave;
                    Timer = 0f;
                }
                break;
            case Leave:
                NPC.direction = nearest.Center.X >= NPC.Center.X ? -1 : 1;
                NPC.velocity.X = NPC.direction * 1.4f;
                if (NPC.collideX && NPC.velocity.Y == 0f)
                    NPC.velocity.Y = -6f;
                NPC.alpha = (int)Math.Clamp((Timer - (LeaveTicks - 60)) * 255f / 60f, 0f, 255f);
                if (Timer > LeaveTicks)
                    NPC.active = false;
                break;
        }
        NPC.spriteDirection = NPC.direction;
    }

    public override void FindFrame(int frameHeight)
    {
        int frame;
        if (State == Gather)
            frame = 7 + (int)(Main.GameUpdateCount / 40 % 2);
        else if (State == Talk)
            frame = 9;
        else if (Math.Abs(NPC.velocity.X) > 0.1f)
        {
            NPC.frameCounter += Math.Abs(NPC.velocity.X);
            frame = 1 + (int)(NPC.frameCounter / 8.0) % 6;
        }
        else
            frame = 0;
        NPC.frame.Y = frame * frameHeight;
    }
}
