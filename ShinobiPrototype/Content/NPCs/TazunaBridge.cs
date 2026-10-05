using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;

namespace ShinobiPrototype.Content.NPCs;

// Tazuna before the win: stands in his hut by the unfinished bridge, cannot be hurt (like the Dungeon's Old Man)
// and is not a town NPC. WaveBridgeWorld re-places him when missing and removes him once the bridge is finished.
// Shares the town Tazuna's sprite sheet.
public sealed class TazunaBridge : ModNPC
{
    public override string Texture => "ShinobiPrototype/Content/NPCs/Tazuna";

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = NpcSheet.FrameCount;
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers { Hide = true });
    }

    public override void SetDefaults()
    {
        NPC.width = 18;
        NPC.height = 40;
        NPC.aiStyle = -1;
        NPC.friendly = true;
        NPC.dontTakeDamage = true;
        NPC.lifeMax = 250;
        NPC.knockBackResist = 0f;
        NPC.npcSlots = 0f;
        NPC.scale = NpcSheet.HiresScale;
    }

    public override bool CanChat() => true;

    public override bool CheckActive() => false;

    private const int LineCount = 9;   // TazunaBridge.Line1..9

    public override string GetChat()
    {
        Player player = Main.LocalPlayer;
        switch (player.GetModPlayer<StoryPlayer>().WaveStage)
        {
            case WaveStage.FindTazuna when !StoryWorld.MetTazuna:
                StoryWorld.RecordTazunaTalk(false);
                return Loc.Get("TazunaBridge.FirstMeeting");
            case WaveStage.ReportToTazuna:
                StoryWorld.RecordTazunaTalk(true);
                // The third Mist insignia (with the brothers' two): one the Mist ninja dropped by his hut.
                player.QuickSpawnItem(player.GetSource_Misc("TazunaConfession"), ModContent.ItemType<MistInsignia>());
                Main.NewText(Loc.Get("TazunaBridge.RankRaised"), 255, 190, 90);
                return Loc.Get("TazunaBridge.Confession");
        }

        List<string> situational = new();
        if (!Main.dayTime)
            situational.Add(Loc.Get("TazunaBridge.Night"));
        if (Main.raining)
            situational.Add(Loc.Get("TazunaBridge.Rain"));
        switch (player.GetModPlayer<StoryPlayer>().WaveStage)
        {
            case WaveStage.GetStronger:
                situational.Add(Loc.Get("TazunaBridge.GetStronger"));
                break;
            case WaveStage.Lake:
                situational.Add(Loc.Get("TazunaBridge.Lake"));
                break;
            case WaveStage.Bridge:
                situational.Add(Loc.Get("TazunaBridge.Bridge"));
                break;
        }
        if (player.GetModPlayer<MistEncounterPlayer>().SawPreview)
            situational.Add(Loc.Get("TazunaBridge.SawPreview"));
        if (player.CountItem(ModContent.ItemType<MistInsignia>()) >= 3)
            situational.Add(Loc.Get("TazunaBridge.Insignia"));
        return situational.Count > 0 && Main.rand.NextBool()
            ? Main.rand.Next(situational)
            : Loc.Pick("TazunaBridge.Line", LineCount);
    }

    public override void AI()
    {
        NPC.velocity.X = 0f;
        Player nearest = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)];
        NPC.direction = nearest.Center.X >= NPC.Center.X ? 1 : -1;
        NPC.spriteDirection = NPC.direction;
    }

    public override void FindFrame(int frameHeight) => NPC.frame.Y = NpcSheet.IdleFrame * frameHeight;
}
