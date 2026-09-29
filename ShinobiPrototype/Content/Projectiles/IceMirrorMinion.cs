using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.Buffs;

namespace ShinobiPrototype.Content.Projectiles;

// An ice mirror floating behind the player's shoulder that throws a fan of three senbon at the nearest enemy (or the
// player's whip target) about every two-thirds of a second. It never touches enemies itself.
public sealed class IceMirrorMinion : ModProjectile
{
    private const float Range = 700f;
    private const int FireTicks = 40;
    private const float NeedleSpeed = 12f;

    public override string Texture => "ShinobiPrototype/Content/NPCs/IceMirror_0";

    public override void SetStaticDefaults()
    {
        Main.projPet[Type] = true;
        ProjectileID.Sets.MinionSacrificable[Type] = true;
        ProjectileID.Sets.MinionTargettingFeature[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 36;
        Projectile.friendly = false;
        Projectile.minion = true;
        Projectile.minionSlots = 1f;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.netImportant = true;
        Projectile.timeLeft = 2;
    }

    public override bool? CanCutTiles() => false;

    public override bool MinionContactDamage() => false;

    public override void AI()
    {
        Player owner = Main.player[Projectile.owner];
        if (!owner.active || owner.dead)
        {
            owner.ClearBuff(ModContent.BuffType<IceMirrorBuff>());
            return;
        }
        if (owner.HasBuff(ModContent.BuffType<IceMirrorBuff>()))
            Projectile.timeLeft = 2;

        // Hover behind the player's shoulder, mirrors lining up away from the player.
        int index = 0;
        foreach (Projectile other in Main.ActiveProjectiles)
            if (other.owner == Projectile.owner && other.type == Type && other.whoAmI < Projectile.whoAmI)
                index++;
        Vector2 home = owner.Center + new Vector2(-owner.direction * (40f + index * 30f), -50f - index % 2 * 12f);
        Projectile.Center = Vector2.Lerp(Projectile.Center, home, 0.12f);
        Projectile.velocity = Vector2.Zero;
        Lighting.AddLight(Projectile.Center, 0.2f, 0.45f, 0.6f);

        NPC target = FindTarget(owner);
        if (target == null || Projectile.owner != Main.myPlayer || ++Projectile.ai[0] < FireTicks)
            return;
        Projectile.ai[0] = 0f;
        Vector2 aim = Projectile.DirectionTo(target.Center) * NeedleSpeed;
        for (int i = -1; i <= 1; i++)
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, aim.RotatedBy(i * 0.12f),
                ModContent.ProjectileType<SenbonThrown>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1f);
        Projectile.netUpdate = true;
    }

    private NPC FindTarget(Player owner)
    {
        if (owner.HasMinionAttackTargetNPC)
        {
            NPC chosen = Main.npc[owner.MinionAttackTargetNPC];
            if (chosen.CanBeChasedBy(Projectile) && chosen.Distance(Projectile.Center) < Range * 1.5f)
                return chosen;
        }
        NPC best = null;
        float bestDistance = Range;
        foreach (NPC npc in Main.ActiveNPCs)
        {
            float distance = npc.Distance(Projectile.Center);
            if (npc.CanBeChasedBy(Projectile) && distance < bestDistance &&
                Collision.CanHitLine(Projectile.Center, 1, 1, npc.Center, 1, 1))
            {
                bestDistance = distance;
                best = npc;
            }
        }
        return best;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        float bob = (float)System.Math.Sin(Main.GameUpdateCount * 0.06f + Projectile.whoAmI) * 3f;
        Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0f, bob) - Main.screenPosition, null,
            Color.White * 0.9f, 0f, texture.Size() / 2f, 0.6f, SpriteEffects.None);
        return false;
    }
}
