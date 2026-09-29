using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.UI.BigProgressBar;
using ShinobiPrototype.Common;

namespace ShinobiPrototype.Content.NPCs;

public sealed class ZabuzaBossBar : ModBossBar
{
    public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
    {
        bool demon = false;
        int bossType = ModContent.NPCType<ZabuzaBoss>();
        foreach (NPC npc in Main.ActiveNPCs)
            if (npc.type == bossType && ZabuzaCombatRules.InMistPhase(npc.life, npc.lifeMax))
            {
                demon = true;
                break;
            }
        Asset<Texture2D> portrait = ModContent.Request<Texture2D>(demon
            ? "ShinobiPrototype/Content/NPCs/ZabuzaPortraitDemonV4"
            : "ShinobiPrototype/Content/NPCs/ZabuzaPortraitV4");
        iconFrame = new Rectangle(0, 0, ZabuzaCombatRules.PortraitSize,
            ZabuzaCombatRules.PortraitSize);
        return portrait;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams)
    {
        drawParams.IconScale = ZabuzaCombatRules.BossBarIconScale;
        return true;
    }

    // Vanilla bar layout, colors, numeric setting and animation are left intact.
}
