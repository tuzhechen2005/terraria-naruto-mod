using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using ShinobiPrototype.Common.Players;

namespace ShinobiPrototype.Common.Systems;

public sealed class ChakraHud : ModSystem
{
    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name == "Vanilla: Resource Bars");
        if (index < 0)
            index = layers.Count;

        layers.Insert(index, new LegacyGameInterfaceLayer(
            "ShinobiPrototype: Chakra",
            DrawChakra,
            InterfaceScaleType.UI));
    }

    private static bool DrawChakra()
    {
        if (Main.gameMenu || Main.LocalPlayer is null || !Main.LocalPlayer.active)
            return true;

        ChakraPlayer player = Main.LocalPlayer.GetModPlayer<ChakraPlayer>();
        ShinobiClientConfig config = ShinobiClientConfig.Instance;
        int x = 20 + config.ChakraBarOffsetX;
        int y = 70 + config.ChakraBarOffsetY;

        Utils.DrawBorderString(Main.spriteBatch, Loc.Get("Hud.Chakra", player.Chakra, player.MaxChakra), new Vector2(x + 2, y), Color.White, 0.8f);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(x, y + 22, 166, 18), Color.Black * 0.8f);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value,
            new Rectangle(x + 3, y + 25, (int)(160f * player.Chakra / player.MaxChakra), 12), new Color(45, 170, 235));
        DrawLogs(x + 2, y + 43);
        int line = y + 81;
        if (DrawToolStealth(x + 2, line))
            line += 16;
        if (DrawSeals(x + 2, line))
            line += 44;
        if (Main.LocalPlayer.GetModPlayer<DebugGodPlayer>().Enabled)
        {
            Utils.DrawBorderString(Main.spriteBatch, Loc.Get("Hud.GodMode"), new Vector2(x + 2, line), Color.Gold, 0.75f);
            line += 20;
        }
        if (config.QuestTracker)
            DrawTracker(x + 2, line);
        return true;
    }

    // The substitution logs (specs/装备与忍术系统.spec.md): one log per charge, the missing ones dark, the next one
    // filling up from the bottom as it comes back; the key after them.
    private static void DrawLogs(int x, int y)
    {
        SubstitutionPlayer logs = Main.LocalPlayer.GetModPlayer<SubstitutionPlayer>();
        Texture2D log = ModContent.Request<Texture2D>("ShinobiPrototype/Content/Projectiles/SubstitutionLog").Value;
        const float scale = 1f;
        int w = (int)(log.Width * scale), h = (int)(log.Height * scale);
        bool sealedPoints = Main.LocalPlayer.GetModPlayer<JutsuStatusPlayer>().SubstitutionSealed;
        for (int i = 0; i < logs.MaxLogs; i++)
        {
            Vector2 at = new(x + i * (w + 4), y);
            if (i < logs.Logs)
            {
                Main.spriteBatch.Draw(log, at, null, sealedPoints ? new Color(120, 140, 200) : Color.White, 0f, Vector2.Zero, scale,
                    SpriteEffects.None, 0f);
                continue;
            }
            Main.spriteBatch.Draw(log, at, null, Color.Black * 0.6f, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            if (i == logs.Logs)
            {
                // The part already back, from the bottom up.
                int shown = (int)(log.Height * logs.NextLog);
                if (shown > 0)
                    Main.spriteBatch.Draw(log, at + new Vector2(0f, (log.Height - shown) * scale),
                        new Rectangle(0, log.Height - shown, log.Width, shown), Color.White * 0.55f, 0f, Vector2.Zero, scale,
                        SpriteEffects.None, 0f);
            }
        }
        Utils.DrawBorderString(Main.spriteBatch, sealedPoints ? Loc.Get("Hud.SubstitutionSealed") : Loc.Get("Hud.Substitution", ShinobiKeybinds.SubstitutionKeyName()),
            new Vector2(x + logs.MaxLogs * (w + 4) + 4, y + h / 2f - 8f), sealedPoints ? new Color(150, 170, 230) : new Color(200, 170, 110), 0.75f);
    }

    // The ninja tools' stealth while one is in hand (ninja-tools-v1 bar; plain bars without it): filling, and lit when
    // full or when hidden after a blink, meaning the next throw is a stealth throw.
    private static bool DrawToolStealth(int x, int y)
    {
        ToolStealthPlayer tool = Main.LocalPlayer.GetModPlayer<ToolStealthPlayer>();
        if (!tool.HoldingTool)
            return false;
        float meter = Main.LocalPlayer.GetModPlayer<StealthPlayer>().Hidden ? 1f : tool.Meter;
        bool ready = tool.Ready;
        const string ui = "ShinobiPrototype/Assets/UI/";
        if (ModContent.HasAsset(ui + "StealthBarFrame") && ModContent.HasAsset(ui + "StealthBarFill"))
        {
            Texture2D frame = ModContent.Request<Texture2D>(ui + "StealthBarFrame").Value;
            Texture2D fill = ModContent.Request<Texture2D>(ui + "StealthBarFill").Value;
            Main.spriteBatch.Draw(frame, new Vector2(x, y), null, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
            int shown = (int)(fill.Width * meter);
            Vector2 inner = new Vector2(x, y) + StealthFillOffset * 1.5f;
            if (shown > 0)
                Main.spriteBatch.Draw(fill, inner, new Rectangle(0, 0, shown, fill.Height), ready ? Color.White : Color.White * 0.8f, 0f,
                    Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
            if (ready)
                Main.spriteBatch.Draw(fill, inner, null, new Color(255, 255, 255, 0) * (0.3f + 0.2f * (float)System.Math.Sin(Main.GameUpdateCount * 0.2f)),
                    0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
        }
        else
        {
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(x, y + 2, 90, 8), Color.Black * 0.7f);
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(x + 1, y + 3, (int)(88 * meter), 6),
                ready ? new Color(230, 180, 255) : new Color(150, 90, 200));
        }
        Utils.DrawBorderString(Main.spriteBatch, Loc.Get(ready ? "Hud.StealthThrow" : "Hud.Stealth"), new Vector2(x + 100, y - 1),
            ready ? new Color(230, 190, 255) : new Color(160, 130, 190), 0.7f);
        return true;
    }

    // Where the fill sits inside the stealth bar frame art (ninja-tools-v1; adjusted to the delivery).
    private static readonly Vector2 StealthFillOffset = new(4f, 3f);

    // The three seal slots, always in sight in a fight (user, 2026-10-03): each scroll with its key, darkened from the
    // top while it cools down with the seconds left, and a flash the moment it is ready again. Empty slots are skipped;
    // nothing is drawn without any scroll.
    private static bool DrawSeals(int x, int y)
    {
        SealPlayer seals = Main.LocalPlayer.GetModPlayer<SealPlayer>();
        bool any = false;
        const int size = 38;
        for (int i = 0; i < 3; i++)
        {
            int count = 2 + 2 * i;
            if (seals.ScrollFor(count) is not { } scroll)
                continue;
            any = true;
            Rectangle box = new(x + i * (size + 6), y, size, size);
            Texture2D back = ModContent.HasAsset("ShinobiPrototype/Assets/UI/SealSlotBack")
                ? ModContent.Request<Texture2D>("ShinobiPrototype/Assets/UI/SealSlotBack").Value
                : TextureAssets.InventoryBack.Value;
            Main.spriteBatch.Draw(back, box, Color.White);
            Terraria.UI.ItemSlot.DrawItemIcon(scroll.Item, Terraria.UI.ItemSlot.Context.InventoryItem, Main.spriteBatch,
                box.Center.ToVector2(), 0.8f, 28f, Color.White);
            int cooldown = seals.CooldownOf(i);
            if (cooldown > 0 && scroll.CooldownTicks > 0)
            {
                float left = cooldown / (float)scroll.CooldownTicks;
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(box.X, box.Y, box.Width, (int)(box.Height * left)),
                    Color.Black * 0.65f);
                Utils.DrawBorderString(Main.spriteBatch, cooldown >= 60 ? $"{(cooldown + 59) / 60}" : $"{cooldown / 60f:0.0}",
                    box.Center.ToVector2(), Color.White, 0.85f, 0.5f, 0.45f);
            }
            else if (cooldown == 0 && seals.ReadyFlash(i) > 0)
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, box, new Color(255, 230, 150) * (seals.ReadyFlash(i) / 20f * 0.6f));
            Utils.DrawBorderString(Main.spriteBatch, ShinobiKeybinds.SealKeyName(count), new Vector2(box.X + 3, box.Y + 1),
                new Color(255, 225, 150), 0.65f);
        }
        return any;
    }

    private static void DrawTracker(int x, int y)
    {
        Player player = Main.LocalPlayer;
        (string title, Vector2? where) = player.GetModPlayer<StoryPlayer>().Tracker();
        string text = Loc.Get("Hud.Tracker", title);
        if (where is Vector2 at)
        {
            Vector2 d = (at - player.Center) / 16f;
            text += d.Length() < 20f ? Loc.Get("Dir.NearbyParen")
                : Loc.Get("Hud.TrackerAcross", Loc.Get(StoryRules.Direction((int)d.X)), System.Math.Abs((int)d.X)) +
                  (System.Math.Abs(d.Y) > 30f
                      ? Loc.Get("Hud.TrackerDepth", Loc.Get(d.Y < 0 ? "Dir.Up" : "Dir.Down"), System.Math.Abs((int)d.Y)) : "");
        }
        Utils.DrawBorderString(Main.spriteBatch, text, new Vector2(x, y), new Color(255, 230, 160), 0.75f);
    }
}
