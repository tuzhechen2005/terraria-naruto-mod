using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Players;

// /m0 dmg: every hit the local player takes is printed in the chat, to check the damage table
// (specs/敌方伤害标准.spec.md) in play: what hit, the table's number (a projectile's spawn damage times vanilla's 2,
// or an NPC's contact damage), the damage before and after defence, and whether a log took it.
public sealed class DebugDamagePlayer : ModPlayer
{
    public static bool Enabled { get; set; }

    public override void OnHurt(Player.HurtInfo info) => Report(Player, info, null);

    public static void Report(Player player, Player.HurtInfo info, string outcome)
    {
        if (!Enabled || player.whoAmI != Main.myPlayer)
            return;
        string source = Loc.Get("Debug.OtherSource");
        string table = "—";
        int projectile = info.DamageSource.SourceProjectileLocalIndex;
        int npc = info.DamageSource.SourceNPCIndex;
        if (projectile >= 0 && projectile < Main.maxProjectiles && Main.projectile[projectile].active)
        {
            Projectile p = Main.projectile[projectile];
            source = p.ModProjectile?.Name ?? Lang.GetProjectileName(p.type).Value;
            if (p.ModProjectile is Content.Projectiles.JutsuHitbox)
                source += $"({(Content.Projectiles.JutsuKind)(int)p.ai[0]})";
            table = (p.damage * 2).ToString();
        }
        else if (npc >= 0 && npc < Main.maxNPCs && Main.npc[npc].active)
        {
            NPC n = Main.npc[npc];
            source = Loc.Get("Debug.Contact", n.TypeName);
            table = n.damage.ToString();
        }
        string tail = outcome ?? Loc.Get(ChakraRules.WorthALog(info.Damage, player.statLifeMax2) ? "Debug.Hit" : "Debug.HitBelowLog");
        Main.NewText(Loc.Get("Debug.Report", source, table, info.SourceDamage, info.Damage, tail, player.statLife, player.statLifeMax2),
            new Color(255, 170, 120));
    }
}
