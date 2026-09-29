using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Common.Players;

// The cage edge is enforced on the trapped player's own client, where movement is authoritative.
public sealed class MirrorCagePlayer : ModPlayer
{
    private int hurtCooldown;

    public override void PostUpdate()
    {
        if (Player.whoAmI != Main.myPlayer || Player.dead)
            return;
        if (hurtCooldown > 0)
            hurtCooldown--;
        int hakuType = ModContent.NPCType<HakuBoss>();
        foreach (NPC npc in Main.ActiveNPCs)
        {
            if (npc.type != hakuType || npc.target != Player.whoAmI ||
                npc.ModNPC is not HakuBoss haku || !haku.CageBoundaryActive)
                continue;
            Vector2 offset = Player.Center - haku.MirrorCenter;
            if (!WaveDuoRules.PushInsideCage(offset.X, offset.Y, WaveDuoRules.CageRadius,
                out float x, out float y))
                return;
            Player.Center = haku.MirrorCenter + new Vector2(x, y);
            Vector2 inward = -Vector2.Normalize(offset);
            Player.velocity = inward * 6f;
            if (hurtCooldown <= 0)
            {
                hurtCooldown = WaveDuoRules.CageBoundaryHurtCooldown;
                Player.Hurt(PlayerDeathReason.ByCustomReason(NetworkText.FromKey(
                    "Mods.ShinobiPrototype.Dialogue.CageTrapped", Player.name)), WaveDuoRules.CageBoundaryDamage, 0);
            }
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.PlayerControls, number: Player.whoAmI);
            return;
        }
    }
}
