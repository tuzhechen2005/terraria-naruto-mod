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
    public override string Usage => "/m0、/m0 items、/m0 time <day|noon|night|midnight|hh:mm>、/m0 bridge、/m0 preview、/m0 sighting、/m0 senbon、/m0 mist、/m0 brothers、/m0 forest、/m0 squad、/m0 lake、/m0 story <1-5>、/m0 exam [阶段|gate|tower|stadium|academy|hokage]、/m0 epilogue zabuza|haku [距离]、/m0 cheer [首领名] 或 /m0 god [on|off]";
    public override string Description => "领取 M0 测试道具、领取模组全部物品（含开发者之翼），或切换仅限单人的临时测试无敌";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        Player player = caller.Player;
        if (args.Length == 1 && args[0].Equals("items", StringComparison.OrdinalIgnoreCase))
        {
            int given = GiveAllItems(player);
            caller.Reply($"已发放本模组全部 {given} 种物品（含开发者之翼）。", Color.LightGreen);
            return;
        }

        if (args.Length == 1 && args[0].Equals("bridge", StringComparison.OrdinalIgnoreCase))
        {
            if (WaveBridgeWorld.Site is not BridgeSite site)
            {
                caller.Reply("这个世界还没有大桥。", Color.OrangeRed);
                return;
            }
            player.Teleport(new Vector2(site.X(0) * 16f, (site.DeckY - 3) * 16f), TeleportationStyleID.RodOfDiscord);
            int built = WaveBridgeWorld.BuiltVersion;
            caller.Reply($"已传送到桥头。本世界大桥版本：{(built > 0 ? $"v{built}" : "未记录（很旧）")}，当前代码：v{BridgeDesign.Version}" +
                (built == BridgeDesign.Version ? "。" : "——需要新建世界才能看到最新的大桥和小屋。"),
                built == BridgeDesign.Version ? Color.LightGreen : Color.Orange);
            return;
        }

        if (args.Length == 1 && args[0].Equals("preview", StringComparison.OrdinalIgnoreCase))
        {
            if (!WaveBridgeWorld.Site.HasValue)
            {
                caller.Reply("这个世界还没有大桥。", Color.OrangeRed);
                return;
            }
            MistPreviewSystem.Play();
            caller.Reply("在本机重放迷雾预告（不改变世界进度）。", Color.LightGreen);
            return;
        }

        if (args.Length == 1 && args[0].Equals("lake", StringComparison.OrdinalIgnoreCase))
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                caller.Reply("湖边初遇测试只在单人模式可用。", Color.OrangeRed);
                return;
            }
            LakeAmbushSystem.Lake? nearest = null;
            foreach (LakeAmbushSystem.Lake lake in LakeAmbushSystem.Lakes)
                if (nearest is not LakeAmbushSystem.Lake best ||
                    Math.Abs(lake.CenterX * 16f - player.Center.X) < Math.Abs(best.CenterX * 16f - player.Center.X))
                    nearest = lake;
            if (nearest is not LakeAmbushSystem.Lake found)
            {
                caller.Reply("这个世界的地表没有找到合适的湖。", Color.OrangeRed);
                return;
            }
            float shore = (found.CenterX + (player.Center.X < found.CenterX * 16f ? -1 : 1) * (found.Width / 2 + 3)) * 16f;
            player.Teleport(new Vector2(shore, (found.SurfaceY - 6) * 16f), TeleportationStyleID.RodOfDiscord);
            LakeAmbushSystem.Start(found, player);
            caller.Reply($"已传送到湖边（宽 {found.Width} 格）并开始湖边初遇（不检查任务进度；完成后会记录 LakeDone）。",
                Color.LightGreen);
            return;
        }

        if (args.Length == 2 && args[0].Equals("story", StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(args[1], out int stage) && stage is >= 1 and <= 5)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                caller.Reply("设置任务阶段只在单人模式可用。", Color.OrangeRed);
                return;
            }
            StoryWorld.MetTazuna = stage >= 2;
            StoryWorld.DownedDemonBrothers = stage >= 3;
            StoryWorld.TazunaConfessed = stage >= 4;
            StoryWorld.LakeDone = stage >= 5;
            caller.Reply($"任务阶段已设为 {stage}（1 找达兹纳、2 侦察、3 回找达兹纳、4 变强/湖边、5 断桥）。\n" +
                         player.GetModPlayer<StoryPlayer>().CurrentObjective(), Color.LightGreen);
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
            caller.Reply($"已记下“打倒了{boss}”。走进木叶，村民就会庆祝。（传送：/m0 exam hokage）", Color.LightGreen);
            return;
        }

        if (args.Length >= 1 && args[0].Equals("exam", StringComparison.OrdinalIgnoreCase))
        {
            Exam(caller, player, args.Length > 1 ? args[1] : null);
            return;
        }

        if (args.Length == 1 && args[0].Equals("squad", StringComparison.OrdinalIgnoreCase))
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                caller.Reply("召唤考生小队只在单人模式可用。", Color.OrangeRed);
                return;
            }
            Common.Systems.DeathForestSystem.SendSquad(player, ModContent.NPCType<Content.NPCs.ForestCanopyCandidate>());
            caller.Reply("一队考生正从屏幕外赶来（要计入进度，需处在第二试抢卷阶段）。", Color.LightGreen);
            return;
        }

        if (args.Length == 1 && args[0].Equals("forest", StringComparison.OrdinalIgnoreCase))
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                caller.Reply("召唤采药少年只在单人模式可用。", Color.OrangeRed);
                return;
            }
            NPC.NewNPC(player.GetSource_Misc("ShinobiM0"), (int)player.Center.X + player.direction * 14 * 16,
                (int)player.Bottom.Y, ModContent.NPCType<Content.NPCs.HakuForest>());
            caller.Reply("前方的林子里，有个少年在采药。", Color.LightGreen);
            return;
        }

        if (args.Length == 1 && args[0].Equals("brothers", StringComparison.OrdinalIgnoreCase))
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                caller.Reply("召唤鬼之兄弟只在单人模式可用。", Color.OrangeRed);
                return;
            }
            NPC.NewNPC(player.GetSource_Misc("ShinobiM0"), (int)player.Center.X - 20 * 16, (int)player.Bottom.Y,
                ModContent.NPCType<Content.NPCs.DemonBrotherGozu>());
            caller.Reply("鬼之兄弟从两侧包抄过来了（左侧伽乌斯，右侧美伊兹）。", Color.LightGreen);
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
            caller.Reply("在本机播放一次雾中剪影出没（不改变进度）。在断口附近按正式位置出现，否则出现在你面前约 15 格。", Color.LightGreen);
            return;
        }

        if (args.Length == 1 && args[0].Equals("senbon", StringComparison.OrdinalIgnoreCase))
        {
            player.GetModPlayer<MistEncounterPlayer>().ThrowWarning(WaveBridgeWorld.Site);
            caller.Reply("在本机播放一次千本警告（不改变进度）。在断口附近按正式位置出现，否则从你面前约 15 格处飞来。", Color.LightGreen);
            return;
        }

        if (args.Length > 0 && args[0].Equals("time", StringComparison.OrdinalIgnoreCase))
        {
            SetTime(caller, args);
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
                caller.Reply("测试无敌只在单人模式可用。", Color.OrangeRed);
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
                ? "测试无敌已开启：伤害将被完全忽略。输入 /m0 god off 可关闭。"
                : "测试无敌已关闭：伤害恢复正常。", enabled ? Color.LightGreen : Color.Orange);
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
            caller.Reply("尾声测试只在单人模式可用。", Color.OrangeRed);
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
        caller.Reply($"尾声开始：{(zabuzaWalks ? "再不斩" : "白")}说完话后走向 {distance} 格外的{(zabuzaWalks ? "白" : "再不斩")}。" +
                     "结束后卡卡西会出现（考试进度已重置为等推荐）。", Color.LightGreen);
    }

    // The Chunin Exams: show the stage, jump to a stage (single player), or go to one of the exam places.
    private static void Exam(CommandCaller caller, Player player, string arg)
    {
        ChuninExamPlayer exam = player.GetModPlayer<ChuninExamPlayer>();
        if (arg == null)
        {
            string sites = string.Join("、", ExamSiteWorld.All().Select(s => $"{s.Kind}@{s.CenterX},{s.GroundY}"));
            caller.Reply($"中忍考试：阶段 {exam.Stage}；场地 {(sites.Length > 0 ? sites : "无（旧世界或没有木叶）")}，" +
                         $"版本 v{ExamSiteWorld.BuiltVersion}/v{ExamSiteDesign.Version}。\n可用阶段：" +
                         string.Join(" ", Enum.GetNames<ExamStage>()), Color.LightSkyBlue);
            return;
        }
        Vector2? target = arg.ToLowerInvariant() switch
        {
            "gate" => SiteTop(ExamSiteWorld.Gate),
            "tower" => SiteTop(ExamSiteWorld.Tower),
            "stadium" => SiteTop(ExamSiteWorld.Stadium),
            "academy" => BuildingDoor("忍者学校"),
            "hokage" => KonohaWorld.HokageFeet - new Vector2(0f, 24f),
            _ => null,
        };
        if (target is Vector2 where)
        {
            player.Teleport(where - new Vector2(player.width / 2f, player.height), TeleportationStyleID.RodOfDiscord);
            caller.Reply($"已传送到 {arg}。", Color.LightGreen);
            return;
        }
        if (arg is "gate" or "tower" or "stadium" or "academy" or "hokage")
        {
            caller.Reply("这个世界没有这个场地（需要新建世界）。", Color.OrangeRed);
            return;
        }
        if (!Enum.TryParse(arg, true, out ExamStage stage))
        {
            caller.Reply("没有这个阶段。输入 /m0 exam 查看可用阶段。", Color.OrangeRed);
            return;
        }
        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            caller.Reply("设置考试阶段只在单人模式可用。", Color.OrangeRed);
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
        caller.Reply($"考试阶段已设为 {stage}，当前判定为 {exam.Stage}。\n" + player.GetModPlayer<StoryPlayer>().CurrentObjective(),
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
            caller.Reply("调整时间只在单人模式可用。", Color.OrangeRed);
            return;
        }
        if (args.Length != 2 || !DebugModeRules.TryParseTime(args[1], out bool dayTime, out double time))
        {
            caller.Reply("用法：/m0 time day（4:30）| noon（12:00）| night（19:30）| midnight（0:00）| hh:mm（24 小时制）", Color.OrangeRed);
            return;
        }

        Main.dayTime = dayTime;
        Main.time = time;
        caller.Reply($"时间已调整为 {args[1]}（{(dayTime ? "白天" : "夜晚")}）。", Color.LightGreen);
    }

    // What the sea mist is doing right now, to tell a design problem from a setting or state problem.
    private static string MistDiagnostics(Player player)
    {
        if (WaveBridgeWorld.Site is null)
            return "海雾诊断：这个世界没有大桥。";
        float distance = WaveBridgeWorld.DistanceToBridgeTiles(player.Center);
        float setting = ShinobiClientConfig.Instance.SeaFogStrength / 100f;
        return "海雾诊断：" +
               $"距大桥 {distance:0} 格（{BridgeRules.FogReachTiles:0} 格外无雾）；" +
               $"海雾{(WaveBridgeWorld.MistActive ? "生效中" : "未生效（大桥已完工或波之国已完成）")}；" +
               $"{(Main.dayTime ? "白天" : "夜晚")}{(Main.raining ? "、下雨" : "")}；" +
               $"浓度设置 {setting:P0}；当前雾密度 {SeaMistSystem.Density:0.00}。";
    }
}
