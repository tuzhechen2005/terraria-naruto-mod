using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Systems;

// Blueprint flow: a local outline preview, a confirm-by-using-again window, and the build request, which the
// server performs in multiplayer.
public sealed class BridgeBlueprintNet : ModSystem
{
    private const int OutlineTicks = 600;

    private static BridgeSite? outline;
    private static BridgeDesign outlineDesign;
    private static bool outlineBlocked;
    private static int outlineTimer;

    public static void ShowOutline(BridgeSite site, bool blocked)
    {
        outline = site;
        outlineDesign = BridgeBuilder.Design(site, StoryWorld.WaveComplete);
        outlineBlocked = blocked;
        outlineTimer = OutlineTicks;
    }

    public static bool IsConfirming(BridgeSite site) =>
        outline is BridgeSite shown && !outlineBlocked && outlineTimer > 0 &&
        shown.ShoreX == site.ShoreX && shown.Dir == site.Dir && shown.WaterY == site.WaterY;

    public static void RequestBuild(BridgeSite site)
    {
        outline = null;
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            ModPacket packet = ModContent.GetInstance<ShinobiPrototype>().GetPacket();
            packet.Write((byte)ShinobiPrototype.Packet.BuildBridge);
            packet.Write(site.ShoreX);
            packet.Write((sbyte)site.Dir);
            packet.Write(site.WaterY);
            packet.Send();
            return;
        }
        Report(WaveBridgeWorld.TryBuildFromBlueprint(site, out string reason), reason, -1);
    }

    public static void BuildOnServer(BridgeSite site, int requester) =>
        Report(WaveBridgeWorld.TryBuildFromBlueprint(site, out string reason), reason, requester);

    private static void Report(bool built, string reason, int requester)
    {
        string text = built ? "达兹纳的大桥立起来了——桥头小屋里，造桥工正等着你。" : reason;
        Color color = built ? new Color(150, 220, 255) : new Color(250, 150, 100);
        if (Main.netMode == NetmodeID.Server)
            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(text), color, requester);
        else
            Main.NewText(text, color);
    }

    public override void PostUpdateEverything()
    {
        if (outlineTimer > 0 && --outlineTimer == 0)
            outline = null;
    }

    public override void OnWorldUnload() => outline = null;

    // Every tile of the design as a translucent square, so the player sees the whole bridge, hut and island
    // before committing; red when something blocks the build.
    public override void PostDrawTiles()
    {
        if (outline is not BridgeSite site)
            return;
        Color color = (outlineBlocked ? new Color(255, 80, 80) : new Color(90, 255, 140)) *
                      (0.35f + 0.15f * (float)System.Math.Sin(Main.GameUpdateCount * 0.1f));
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState,
            DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        foreach (Cell cell in outlineDesign.Cells)
            DrawTile(site.X(cell.Offset), cell.Y, cell.Scaffold ? color * 0.5f : color);
        Main.spriteBatch.End();
    }

    private static void DrawTile(int x, int y, Color color)
    {
        Rectangle rect = new((int)(x * 16 - Main.screenPosition.X), (int)(y * 16 - Main.screenPosition.Y), 16, 16);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, color);
    }
}
