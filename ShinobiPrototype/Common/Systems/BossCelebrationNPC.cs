using Terraria;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Common.Systems;

// Notes, on each player's own client, every boss that falls with that player in the fight, so the Leaf can cheer it
// the next time they come home (StoryPlayer.QueueCelebration).
public sealed class BossCelebrationNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    private bool hitByLocalPlayer;

    public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
    {
        if (player.whoAmI == Main.myPlayer)
            hitByLocalPlayer = true;
    }

    public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        if (projectile.owner == Main.myPlayer && projectile.friendly)
            hitByLocalPlayer = true;
    }

    public override void HitEffect(NPC npc, NPC.HitInfo hit)
    {
        if (npc.life > 0 || !npc.boss || !hitByLocalPlayer || Main.dedServ)
            return;
        bool wave = npc.type == ModContent.NPCType<ZabuzaBoss>() || npc.type == ModContent.NPCType<HakuBoss>();
        Main.LocalPlayer.GetModPlayer<StoryPlayer>().QueueCelebration(wave ? StoryPlayer.WaveDuoName : Lang.GetNPCNameValue(npc.type));
    }
}
