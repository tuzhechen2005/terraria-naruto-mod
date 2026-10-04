using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;

namespace ShinobiPrototype.Content.Commands;

public sealed class M0Command : ModCommand
{
    public override CommandType Type => CommandType.Chat;
    public override string Command => "m0";
    public override string Usage => Loc.Get("M0.Usage");
    public override string Description => Loc.Get("M0.Description");

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        Player player = caller.Player;
        if (args.Length == 1 && args[0].Equals("items", StringComparison.OrdinalIgnoreCase))
        {
            int given = GiveAllItems(player);
            caller.Reply(Loc.Get("M0.ItemsGiven", given), Color.LightGreen);
            return;
        }

        if (args.Length == 1 && args[0].Equals("tools", StringComparison.OrdinalIgnoreCase))
        {
            player.QuickSpawnItem(player.GetSource_GiftOrReward(), ModContent.ItemType<Items.NinjaTools.Shuriken>());
            player.QuickSpawnItem(player.GetSource_GiftOrReward(), ModContent.ItemType<Items.NinjaTools.PaperBomb>(), 99);
            caller.Reply(Loc.Get("M0.ToolsGiven"), Color.LightGreen);
            return;
        }

        if (args.Length == 1 && args[0].Equals("seals", StringComparison.OrdinalIgnoreCase))
        {
            foreach (int type in new[] { ModContent.ItemType<Items.Jutsu.ScrollClone>(), ModContent.ItemType<Items.Jutsu.ScrollFireball>(),
                         ModContent.ItemType<Items.Jutsu.ScrollChidori>() })
                player.QuickSpawnItem(player.GetSource_GiftOrReward(), type);
            caller.Reply(Loc.Get("M0.SealsGiven", Common.Systems.ShinobiKeybinds.SealKeyName(2), Common.Systems.ShinobiKeybinds.SealKeyName(4), Common.Systems.ShinobiKeybinds.SealKeyName(6)), Color.LightGreen);
            return;
        }

        if (args.Length == 1 && args[0].Equals("bridge", StringComparison.OrdinalIgnoreCase))
        {
            if (WaveBridgeWorld.Site is not BridgeSite site)
            {
                caller.Reply(Loc.Get("M0.NoBridge"), Color.OrangeRed);
                return;
            }
            player.Teleport(new Vector2(site.X(0) * 16f, (site.DeckY - 3) * 16f), TeleportationStyleID.RodOfDiscord);
            int built = WaveBridgeWorld.BuiltVersion;
            caller.Reply(Loc.Get("M0.BridgeTeleported", built > 0 ? $"v{built}" : Loc.Get("M0.BridgeVersionUnknown"), BridgeDesign.Version) +
                (built == BridgeDesign.Version ? Loc.Get("M0.BridgeCurrent") : Loc.Get("M0.BridgeOutdated")),
                built == BridgeDesign.Version ? Color.LightGreen : Color.Orange);
            return;
        }

        if (args.Length == 1 && args[0].Equals("preview", StringComparison.OrdinalIgnoreCase))
        {
            if (!WaveBridgeWorld.Site.HasValue)
            {
                caller.Reply(Loc.Get("M0.NoBridge"), Color.OrangeRed);
                return;
            }
            MistPreviewSystem.Play();
            caller.Reply(Loc.Get("M0.PreviewReplayed"), Color.LightGreen);
            return;
        }

        if (args.Length == 1 && args[0].Equals("lake", StringComparison.OrdinalIgnoreCase))
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                caller.Reply(Loc.Get("M0.LakeSingle"), Color.OrangeRed);
                return;
            }
            LakeAmbushSystem.Lake? nearest = null;
            foreach (LakeAmbushSystem.Lake lake in LakeAmbushSystem.Lakes)
                if (nearest is not LakeAmbushSystem.Lake best ||
                    Math.Abs(lake.CenterX * 16f - player.Center.X) < Math.Abs(best.CenterX * 16f - player.Center.X))
                    nearest = lake;
            if (nearest is not LakeAmbushSystem.Lake found)
            {
                caller.Reply(Loc.Get("M0.NoLake"), Color.OrangeRed);
                return;
            }
            float shore = (found.CenterX + (player.Center.X < found.CenterX * 16f ? -1 : 1) * (found.Width / 2 + 3)) * 16f;
            player.Teleport(new Vector2(shore, (found.SurfaceY - 6) * 16f), TeleportationStyleID.RodOfDiscord);
            LakeAmbushSystem.Start(found, player);
            caller.Reply(Loc.Get("M0.LakeStarted", found.Width),
                Color.LightGreen);
            return;
        }

        if (args.Length == 2 && args[0].Equals("story", StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(args[1], out int stage) && stage is >= 1 and <= 5)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                caller.Reply(Loc.Get("M0.StorySingle"), Color.OrangeRed);
                return;
            }
            StoryWorld.MetTazuna = stage >= 2;
            StoryWorld.DownedDemonBrothers = stage >= 3;
            StoryWorld.TazunaConfessed = stage >= 4;
            StoryWorld.LakeDone = stage >= 5;
            caller.Reply(Loc.Get("M0.StorySet", stage) + "\n" + player.GetModPlayer<StoryPlayer>().CurrentObjective(), Color.LightGreen);
            return;
        }

        if (args.Length >= 1 && args[0].Equals("epilogue", StringComparison.OrdinalIgnoreCase))
        {
            Epilogue(caller, player, args);
            return;
        }

        if (args.Length >= 1 && args[0].Equals("cheer", StringComparison.OrdinalIgnoreCase))
        {
            StoryPlayer story = player.GetModPlayer<StoryPlayer>();
            string boss = args.Length > 1 ? string.Join(" ", args[1..]) : StoryPlayer.WaveDuoName;
            if (boss == StoryPlayer.WaveDuoName)
                story.ResetWelcomeForTesting();
            story.QueueCelebration(boss);
            caller.Reply(Loc.Get("M0.CheerQueued", boss == StoryPlayer.WaveDuoName ? Loc.Get("Story.WaveDuoName") : boss), Color.LightGreen);
            return;
        }

        if (args.Length >= 1 && args[0].Equals("exam", StringComparison.OrdinalIgnoreCase))
        {
            Exam(caller, player, args.Length > 1 ? args[1] : null);
            return;
        }

        if (args.Length == 1 && args[0].Equals("orochimaru", StringComparison.OrdinalIgnoreCase))
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                caller.Reply(Loc.Get("M0.OrochimaruSingle"), Color.OrangeRed);
                return;
            }
            Vector2 at = player.Bottom + new Vector2(player.direction * 30 * 16f, 0f);
            if (Common.GroundSpot.TryNear(at, 28, 56, out Vector2 ground))
                at = ground;
            NPC.NewNPC(player.GetSource_Misc("ShinobiM0"), (int)at.X, (int)at.Y, ModContent.NPCType<Content.NPCs.OrochimaruDisguise>());
            caller.Reply(Loc.Get("M0.OrochimaruComing"), Color.LightGreen);
            return;
        }

        if (args.Length == 1 && args[0].Equals("squad", StringComparison.OrdinalIgnoreCase))
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                caller.Reply(Loc.Get("M0.SquadSingle"), Color.OrangeRed);
                return;
            }
            Common.Systems.DeathForestSystem.SendSquad(player, ModContent.NPCType<Content.NPCs.ForestCanopyCandidate>());
            caller.Reply(Loc.Get("M0.SquadComing"), Color.LightGreen);
            return;
        }

        if (args.Length == 1 && args[0].Equals("forest", StringComparison.OrdinalIgnoreCase))
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                caller.Reply(Loc.Get("M0.ForestSingle"), Color.OrangeRed);
                return;
            }
            NPC.NewNPC(player.GetSource_Misc("ShinobiM0"), (int)player.Center.X + player.direction * 14 * 16,
                (int)player.Bottom.Y, ModContent.NPCType<Content.NPCs.HakuForest>());
            caller.Reply(Loc.Get("M0.ForestComing"), Color.LightGreen);
            return;
        }

        if (args.Length == 1 && args[0].Equals("brothers", StringComparison.OrdinalIgnoreCase))
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                caller.Reply(Loc.Get("M0.BrothersSingle"), Color.OrangeRed);
                return;
            }
            NPC.NewNPC(player.GetSource_Misc("ShinobiM0"), (int)player.Center.X - 20 * 16, (int)player.Bottom.Y,
                ModContent.NPCType<Content.NPCs.DemonBrotherGozu>());
            caller.Reply(Loc.Get("M0.BrothersComing"), Color.LightGreen);
            return;
        }

        if (args.Length == 1 && args[0].Equals("mist", StringComparison.OrdinalIgnoreCase))
        {
            caller.Reply(MistDiagnostics(player), Color.LightSkyBlue);
            return;
        }

        if (args.Length == 1 && args[0].Equals("sighting", StringComparison.OrdinalIgnoreCase))
        {
            MistSightingSystem.StartSighting(WaveBridgeWorld.Site, player, withWhisper: true);
            caller.Reply(Loc.Get("M0.SightingPlayed"), Color.LightGreen);
            return;
        }

        if (args.Length == 1 && args[0].Equals("senbon", StringComparison.OrdinalIgnoreCase))
        {
            player.GetModPlayer<MistEncounterPlayer>().ThrowWarning(WaveBridgeWorld.Site);
            caller.Reply(Loc.Get("M0.SenbonPlayed"), Color.LightGreen);
            return;
        }

        if (args.Length > 0 && args[0].Equals("time", StringComparison.OrdinalIgnoreCase))
        {
            SetTime(caller, args);
            return;
        }

        // Checking the damage table in play (specs/敌方伤害标准.spec.md): print every hit taken; switch the logs off.
        if (args.Length is 1 or 2 && args[0].Equals("dmg", StringComparison.OrdinalIgnoreCase))
        {
            DebugDamagePlayer.Enabled = args.Length == 2 ? !args[1].Equals("off", StringComparison.OrdinalIgnoreCase) : !DebugDamagePlayer.Enabled;
            caller.Reply(DebugDamagePlayer.Enabled
                ? Loc.Get("M0.DmgOn")
                : Loc.Get("M0.DmgOff"), Color.LightGreen);
            return;
        }

        if (args.Length == 2 && args[0].Equals("logs", StringComparison.OrdinalIgnoreCase) &&
            (args[1].Equals("off", StringComparison.OrdinalIgnoreCase) || args[1].Equals("on", StringComparison.OrdinalIgnoreCase)))
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                caller.Reply(Loc.Get("M0.LogsSingle"), Color.OrangeRed);
                return;
            }
            SubstitutionPlayer.DebugLogsOff = args[1].Equals("off", StringComparison.OrdinalIgnoreCase);
            caller.Reply(SubstitutionPlayer.DebugLogsOff
                ? Loc.Get("M0.LogsOff")
                : Loc.Get("M0.LogsOn"), Color.LightGreen);
            return;
        }

        if (args.Length > 0)
        {
            if (!args[0].Equals("god", StringComparison.OrdinalIgnoreCase) || args.Length > 2)
            {
                caller.Reply(Usage, Color.OrangeRed);
                return;
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                caller.Reply(Loc.Get("M0.GodSingle"), Color.OrangeRed);
                return;
            }

            DebugGodPlayer debug = player.GetModPlayer<DebugGodPlayer>();
            if (!DebugModeRules.TryResolveGodMode(debug.Enabled,
                args.Length == 2 ? args[1] : null, out bool enabled))
            {
                caller.Reply(Usage, Color.OrangeRed);
                return;
            }

            debug.SetEnabled(enabled);
            caller.Reply(enabled
                ? Loc.Get("M0.GodOn")
                : Loc.Get("M0.GodOff"), enabled ? Color.LightGreen : Color.Orange);
            return;
        }

        player.QuickSpawnItem(player.GetSource_Misc("ShinobiM0"), ModContent.ItemType<TrainingKunai>());
        player.QuickSpawnItem(player.GetSource_Misc("ShinobiM0"), ModContent.ItemType<TrainingRasengan>());
        player.QuickSpawnItem(player.GetSource_Misc("ShinobiM0"), ModContent.ItemType<M0ZabuzaChallengeScroll>());
    }

    // Every item this mod adds, for in-game testing. Legacy items that turn into their replacements are skipped.
    private int GiveAllItems(Player player)
    {
        int given = 0;
        foreach (ModItem item in Mod.GetContent<ModItem>())
        {
            if (item is MissionScroll or HakuChallengeScroll)
                continue;

            int stack = item switch
            {
                ChakraCrystal => ChakraRules.MaxCrystals,
                ChakraPill => 20,
                MistInsignia => 3,
                _ => 1,
            };
            player.QuickSpawnItem(player.GetSource_Misc("ShinobiM0"), item.Type, stack);
            given++;
        }
        return given;
    }

    // The Wave epilogue without the fight: both bodies by the player (the one who walks at the player's feet, the
    // other `distance` tiles ahead), Wave Country marked done and the exam reset to Kakashi's recommendation, so the
    // whole scene plays: the lines, the walk, the snow, then Kakashi with the ride home.
    private static void Epilogue(CommandCaller caller, Player player, string[] args)
    {
        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            caller.Reply(Loc.Get("M0.EpilogueSingle"), Color.OrangeRed);
            return;
        }
        bool zabuzaWalks = args.Length < 2 || !args[1].Equals("haku", StringComparison.OrdinalIgnoreCase);
        int distance = args.Length > 2 && int.TryParse(args[2], out int d) ? Math.Clamp(d, 3, 200) : 18;
        int corpse = ModContent.ProjectileType<Content.Projectiles.WaveCorpse>();
        foreach (Projectile old in Main.ActiveProjectiles)
            if (old.type == corpse)
                old.Kill();
        int walker = zabuzaWalks ? Content.Projectiles.WaveCorpse.Zabuza : Content.Projectiles.WaveCorpse.Haku;
        int dir = player.direction;
        var source = player.GetSource_Misc("ShinobiM0");
        Projectile.NewProjectile(source, player.Center, Vector2.Zero, corpse, 0, 0f, Main.myPlayer, walker, dir);
        Projectile.NewProjectile(source, player.Center + new Vector2(dir * distance * 16f, 0f), Vector2.Zero, corpse, 0, 0f,
            Main.myPlayer, 1 - walker, -dir);
        StoryWorld.CompleteWave();
        player.GetModPlayer<ChuninExamPlayer>().SetStageForTesting(ExamStage.Recommend);
        WaveEpilogueSystem.Begin(hakuFellFirst: zabuzaWalks, player.Center);
        caller.Reply(Loc.Get(zabuzaWalks ? "M0.EpilogueZabuza" : "M0.EpilogueHaku", distance), Color.LightGreen);
    }

    // The Chunin Exams: show the stage, jump to a stage (single player), or go to one of the exam places.
    private static void Exam(CommandCaller caller, Player player, string arg)
    {
        ChuninExamPlayer exam = player.GetModPlayer<ChuninExamPlayer>();
        if (arg == null)
        {
            string sites = string.Join(", ", ExamSiteWorld.All().Select(s => $"{s.Kind}@{s.CenterX},{s.GroundY}"));
            caller.Reply(Loc.Get("M0.ExamStatus", exam.Stage, sites.Length > 0 ? sites : Loc.Get("M0.ExamNoSites"),
                ExamSiteWorld.BuiltVersion, ExamSiteDesign.Version, string.Join(" ", Enum.GetNames<ExamStage>())), Color.LightSkyBlue);
            return;
        }
        if (arg.Equals("rebuild", StringComparison.OrdinalIgnoreCase))
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
                caller.Reply(Loc.Get("M0.RebuildSingle"), Color.OrangeRed);
            else if (ExamSiteWorld.RebuildForest())
                caller.Reply(Loc.Get("M0.Rebuilt", ExamSiteDesign.Version), Color.LightGreen);
            else
                caller.Reply(Loc.Get("M0.RebuildFailed"), Color.OrangeRed);
            return;
        }
        Vector2? target = arg.ToLowerInvariant() switch
        {
            "tree" => SiteTop(ExamSiteWorld.HollowTree),
            "rain" => ExamSiteWorld.RainClearing,
            "gate" => SiteTop(ExamSiteWorld.Gate),
            "tower" => SiteTop(ExamSiteWorld.Tower),
            "stadium" => SiteTop(ExamSiteWorld.Stadium),
            "academy" => BuildingDoor(KonohaBuildings.Academy),
            "hokage" => KonohaWorld.HokageFeet - new Vector2(0f, 24f),
            _ => null,
        };
        if (target is Vector2 where)
        {
            player.Teleport(where - new Vector2(player.width / 2f, player.height), TeleportationStyleID.RodOfDiscord);
            caller.Reply(Loc.Get("M0.Teleported", arg), Color.LightGreen);
            return;
        }
        if (arg is "gate" or "tower" or "stadium" or "academy" or "hokage" or "tree" or "rain")
        {
            caller.Reply(Loc.Get("M0.NoSite"), Color.OrangeRed);
            return;
        }
        if (!Enum.TryParse(arg, true, out ExamStage stage))
        {
            caller.Reply(Loc.Get("M0.NoStage"), Color.OrangeRed);
            return;
        }
        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            caller.Reply(Loc.Get("M0.ExamSingle"), Color.OrangeRed);
            return;
        }
        if (stage > ExamStage.Recommend)
            StoryWorld.CompleteWave();
        StoryWorld.DownedGaara = stage == ExamStage.Done;
        exam.SetStageForTesting(stage);
        if (stage == ExamStage.Written && !player.HasItem(ModContent.ItemType<ExamAdmissionScroll>()))
            player.QuickSpawnItem(player.GetSource_Misc("ShinobiM0"), ModContent.ItemType<ExamAdmissionScroll>());
        if (stage == ExamStage.ForestHunt && !player.HasItem(ModContent.ItemType<HeavenScroll>()))
            player.QuickSpawnItem(player.GetSource_Misc("ShinobiM0"), ModContent.ItemType<HeavenScroll>());
        caller.Reply(Loc.Get("M0.ExamSet", stage, exam.Stage) + "\n" + player.GetModPlayer<StoryPlayer>().CurrentObjective(),
            Color.LightGreen);
    }

    private static Vector2? SiteTop(ExamSite? site) =>
        site is ExamSite s ? new Vector2((s.CenterX + 0.5f) * 16f, s.GroundY * 16f - 8f) : null;

    // Just outside the building's west wall, on the ground.
    private static Vector2? BuildingDoor(string name)
    {
        if (KonohaWorld.Site is not KonohaSite site)
            return null;
        foreach (KBuilding building in KonohaWorld.Design.Buildings)
            if (building.Name == name)
                return new Vector2((site.X(building.X0) + 2.5f) * 16f, site.GroundY * 16f - 8f);
        return null;
    }

    private static void SetTime(CommandCaller caller, string[] args)
    {
        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            caller.Reply(Loc.Get("M0.TimeSingle"), Color.OrangeRed);
            return;
        }
        if (args.Length != 2 || !DebugModeRules.TryParseTime(args[1], out bool dayTime, out double time))
        {
            caller.Reply(Loc.Get("M0.TimeUsage"), Color.OrangeRed);
            return;
        }

        Main.dayTime = dayTime;
        Main.time = time;
        caller.Reply(Loc.Get("M0.TimeSet", args[1], Loc.Get(dayTime ? "M0.Day" : "M0.Night")), Color.LightGreen);
    }

    // What the sea mist is doing right now, to tell a design problem from a setting or state problem.
    private static string MistDiagnostics(Player player)
    {
        if (WaveBridgeWorld.Site is null)
            return Loc.Get("M0.MistNoBridge");
        float distance = WaveBridgeWorld.DistanceToBridgeTiles(player.Center);
        float setting = ShinobiClientConfig.Instance.SeaFogStrength / 100f;
        return Loc.Get("M0.MistReport", distance.ToString("0"), BridgeRules.FogReachTiles.ToString("0"),
            Loc.Get(WaveBridgeWorld.MistActive ? "M0.MistOn" : "M0.MistOff"),
            Loc.Get(Main.dayTime ? "M0.Day" : "M0.Night") + (Main.raining ? Loc.Get("M0.Raining") : ""),
            setting.ToString("P0"), SeaMistSystem.Density.ToString("0.00"));
    }
}
