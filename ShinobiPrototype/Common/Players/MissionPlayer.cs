using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ShinobiPrototype.Content.Items.Tier1;
using ShinobiPrototype.Content.NPCs;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Common.Players;

// This character's mission from Iruka's desk (MissionRules): what it is, how far along, and whether it is done and
// waiting to be reported. Missions are carried out on the client; the reward is paid at the desk.
public sealed class MissionPlayer : ModPlayer
{
    // What a gather mission asks for, and how many.
    private static readonly (int Item, int Count)[] Gathers =
    {
        (ItemID.Daybloom, 5), (ItemID.Mushroom, 10), (ItemID.Gel, 25), (ItemID.Cobweb, 20), (ItemID.Acorn, 10),
        (ItemID.FallenStar, 3), (ItemID.Blinkroot, 3),
    };

    public MissionKind Kind { get; private set; }
    public bool Done { get; private set; }
    private MissionKind last;
    private int target;      // the letter's addressee (NPC type), or the gathered item
    private int count;       // how many to gather
    private int progress;    // ronin beaten on patrol
    private bool far;
    private int tora = -1;

    public string Describe()
    {
        if (Done)
            return Loc.Get("Mission.ReportBack");
        return Kind switch
        {
            MissionKind.Tora => Loc.Get("Mission.Tora.Task"),
            MissionKind.Letter => Loc.Get("Mission.Letter.Task", Lang.GetNPCNameValue(target)),
            MissionKind.Gather => Loc.Get("Mission.Gather.Task", count, Lang.GetItemNameValue(target), Player.CountItem(target)),
            MissionKind.Patrol => Loc.Get("Mission.Patrol.Task", MissionRules.PatrolRonin, progress),
            _ => "",
        };
    }

    // At the desk: take a new mission, hand over what was gathered, or be paid. Returns Iruka's answer.
    public string AtDesk()
    {
        if (Kind == MissionKind.None)
            return Take();
        if (Kind == MissionKind.Gather && !Done && Player.CountItem(target) >= count)
        {
            for (int i = 0; i < count; i++)
                Player.ConsumeItem(target);
            Done = true;
        }
        return Done ? Pay() : Describe();
    }

    private string Take(MissionKind kind = MissionKind.None)
    {
        Kind = kind != MissionKind.None ? kind : MissionRules.Next(last, Main.rand.NextFloat());
        Done = false;
        progress = 0;
        far = false;
        switch (Kind)
        {
            case MissionKind.Letter:
                if (!PickAddressee())
                    Kind = MissionKind.Gather;
                break;
            case MissionKind.Tora:
                if (!LoseTora())
                    Kind = MissionKind.Gather;
                break;
        }
        if (Kind == MissionKind.Gather)
            (target, count) = Gathers[Main.rand.Next(Gathers.Length)];
        Main.NewText(Loc.Get("Mission.Taken", Loc.Get($"Mission.{Kind}.Name")), 255, 220, 150);
        return Loc.Get($"Mission.{Kind}.Brief") + "\n\n" + Describe();
    }

    private string Pay()
    {
        (int silver, int tokens) = MissionRules.Reward(Kind, far);
        var source = Player.GetSource_Misc("Mission");
        Player.QuickSpawnItem(source, ItemID.SilverCoin, silver);
        Player.QuickSpawnItem(source, ModContent.ItemType<MissionToken>(), tokens);
        string line = Loc.Get("Mission.Paid", silver, tokens);
        last = Kind;
        Kind = MissionKind.None;
        Done = false;
        Terraria.Audio.SoundEngine.PlaySound(SoundID.Coins, Player.Center);
        return line;
    }

    // Someone living in the world, other than Iruka, gets a letter.
    private bool PickAddressee()
    {
        var people = new System.Collections.Generic.List<NPC>();
        foreach (NPC npc in Main.ActiveNPCs)
            if ((npc.townNPC || npc.type == ModContent.NPCType<TazunaBridge>()) && npc.type != ModContent.NPCType<Iruka>())
                people.Add(npc);
        if (people.Count == 0)
            return false;
        NPC chosen = people[Main.rand.Next(people.Count)];
        target = chosen.type;
        far = chosen.Distance(Player.Center) > MissionRules.FarLetterTiles * 16f;
        return true;
    }

    // Tora hides on the surface 150 to 300 tiles away, only for this player.
    private bool LoseTora()
    {
        if (Player.whoAmI != Main.myPlayer)
            return false;
        for (int tries = 0; tries < 40; tries++)
        {
            int x = (int)(Player.Center.X / 16f) + (Main.rand.NextBool() ? 1 : -1) * Main.rand.Next(MissionRules.ToraMinTiles, MissionRules.ToraMaxTiles + 1);
            if (!WorldGen.InWorld(x, 50, 50))
                continue;
            for (int y = 50; y < Main.worldSurface; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile || !Main.tileSolid[tile.TileType] || tile.LiquidAmount > 0)
                    continue;
                if (Collision.SolidCollision(new Vector2(x * 16f - 12f, y * 16f - 24f), 24, 20))
                    break;
                tora = Projectile.NewProjectile(Player.GetSource_Misc("Mission"), new Vector2(x * 16f + 8f, y * 16f - 10f), Vector2.Zero,
                    ModContent.ProjectileType<LostTora>(), 0, 0f, Player.whoAmI);
                Main.NewText(Loc.Get("Mission.Tora.Direction", Loc.Get(StoryRules.Direction(x - (int)(Player.Center.X / 16f)))), 255, 220, 150);
                return true;
            }
        }
        return false;
    }

    public override void PostUpdate()
    {
        if (Player.whoAmI != Main.myPlayer || Done)
            return;
        switch (Kind)
        {
            case MissionKind.Letter when Player.talkNPC >= 0 && Main.npc[Player.talkNPC].type == target:
                Finish();
                break;
            case MissionKind.Tora when tora >= 0 && Main.projectile[tora] is { active: true } cat && cat.ModProjectile is LostTora &&
                                       cat.Distance(Player.Center) < MissionRules.ToraCatchPx:
                cat.Kill();
                tora = -1;
                CombatText.NewText(Player.getRect(), new Color(255, 200, 120), Loc.Get("Mission.Tora.Caught"));
                Finish();
                break;
            case MissionKind.Tora when tora < 0 || !Main.projectile[tora].active || Main.projectile[tora].ModProjectile is not LostTora:
                if (Main.GameUpdateCount % 60 == 0)   // after a reload, or if she wandered off for good
                    LoseTora();
                break;
        }
    }

    // The local player's blow that fells a ronin counts for the patrol.
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Kind == MissionKind.Patrol && !Done && target.life <= 0 && target.type == ModContent.NPCType<Ronin>() &&
            ++progress >= MissionRules.PatrolRonin)
            Finish();
    }

    public void TakeForTesting(MissionKind kind)
    {
        Kind = MissionKind.None;
        Main.NewText(Take(kind), 255, 220, 150);
    }

    public void FinishForTesting()
    {
        if (Kind != MissionKind.None && !Done)
            Finish();
    }

    private void Finish()
    {
        Done = true;
        Main.NewText(Loc.Get("Mission.Complete", Loc.Get($"Mission.{Kind}.Name")), 255, 220, 150);
    }

    public override void SaveData(TagCompound tag)
    {
        if (Kind == MissionKind.None)
            return;
        tag["missionKind"] = (byte)Kind;
        tag["missionDone"] = Done;
        tag["missionTarget"] = target;
        tag["missionCount"] = count;
        tag["missionProgress"] = progress;
        tag["missionFar"] = far;
    }

    public override void LoadData(TagCompound tag)
    {
        Kind = (MissionKind)tag.GetByte("missionKind");
        Done = tag.GetBool("missionDone");
        target = tag.GetInt("missionTarget");
        count = tag.GetInt("missionCount");
        progress = tag.GetInt("missionProgress");
        far = tag.GetBool("missionFar");
    }
}
