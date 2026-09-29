using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.NPCs;

public sealed class HakuBossBar : ModBossBar
{
    public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
    {
        iconFrame = new Rectangle(0, 0, 40, 40);
        return ModContent.Request<Texture2D>("ShinobiPrototype/Content/NPCs/HakuBoss_Head_Boss");
    }
}
