using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;

namespace ShinobiPrototype.Content.Projectiles;

// The first seal jutsu (specs/装备与忍术系统.spec.md). Art: seal-jutsu-v1 (FxGreatFireball, FxFireBurst, FxChidori,
// FxChidoriTrail); until it is in, dust and vanilla flame stand in.

// 分身术: a shadowy copy of the player at one side (ai[0] = -1 or 1), drawn from the player themself. It takes one hit
// in the player's place (SubstitutionPlayer asks TakeHit first), then goes up in smoke.
public sealed class ShadowClone : ModProjectile
{
    public const int Lifetime = 8 * 60;

    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 42;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = Lifetime;
        Projectile.aiStyle = -1;
    }

    public override void AI()
    {
        Player owner = Main.player[Projectile.owner];
        if (!owner.active || owner.dead)
        {
            Projectile.Kill();
            return;
        }
        if (Projectile.localAI[0] == 0f)
        {
            Projectile.localAI[0] = 1f;
            Puff(Projectile.Center);
        }
        // Beside the player, a step off, drifting a little.
        float side = Projectile.ai[0];
        Vector2 want = owner.Center + new Vector2(side * 40f, (float)Math.Sin((Main.GameUpdateCount + side * 30f) * 0.07f) * 3f);
        Projectile.Center = Vector2.Lerp(Projectile.Center, want, 0.25f);
    }

    // A clone of this player takes the hit: true if there was one.
    public static bool TakeHit(Player player)
    {
        int type = ModContent.ProjectileType<ShadowClone>();
        foreach (Projectile p in Main.ActiveProjectiles)
            if (p.owner == player.whoAmI && p.type == type)
            {
                p.Kill();
                player.SetImmuneTimeForAllTypes(40);
                SoundEngine.PlaySound(SoundID.Item8 with { Pitch = 0.4f }, p.Center);
                CombatText.NewText(player.getRect(), new Color(170, 200, 255), Loc.Get("Seal.CloneCast"));
                return true;
            }
        return false;
    }

    public override void OnKill(int timeLeft) => Puff(Projectile.Center);

    private static void Puff(Vector2 at)
    {
        if (Main.dedServ)
            return;
        for (int i = 0; i < 16; i++)
            Dust.NewDustPerfect(at + Main.rand.NextVector2Circular(12f, 20f), DustID.Smoke, Main.rand.NextVector2Circular(2f, 2f),
                100, default, 1.4f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Player owner = Main.player[Projectile.owner];
        // Fading in its last second.
        float shadow = 0.45f + 0.5f * Math.Max(0f, 1f - Projectile.timeLeft / 60f);
        try
        {
            Main.PlayerRenderer.DrawPlayer(Main.Camera, owner, owner.position + (Projectile.Center - owner.Center), owner.fullRotation,
                owner.fullRotationOrigin, Math.Min(0.95f, shadow));
        }
        catch (Exception)
        {
            // Drawing a player outside its own pass is a best effort; a smoke trail stands in if it fails.
            Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, Vector2.Zero, 150, default, 1f).noGravity = true;
        }
        return false;
    }
}

// 火遁·豪火球之术 (SealRules): out of the mouth it swells in half a second to a ball thirteen tiles across, then rolls
// slowly on through walls for two seconds, burning whatever it touches every quarter second and setting the ground
// below alight, and ends in a great burst that shakes the screen. Art: seal-jutsu-v2 (FxHugeFireball, FxHugeExplosion,
// FxGroundFire), else v1's smaller fireball scaled up.
public sealed class GreatFireball : ModProjectile
{
    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.BallofFire}";

    private int Age => SealRules.FireballLifeTicks - Projectile.timeLeft;
    private float lastFireX = float.NaN;
    private Vector2? burstAt;

    public override void SetDefaults()
    {
        Projectile.width = (int)SealRules.FireballStartPx;
        Projectile.height = (int)SealRules.FireballStartPx;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Generic;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = SealRules.FireballHitCooldownTicks;
        Projectile.timeLeft = SealRules.FireballLifeTicks;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
    }

    // A ball: whatever its corner of the box, only what is inside the circle is burnt.
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        Vector2 nearest = Vector2.Clamp(Projectile.Center, targetHitbox.TopLeft(), targetHitbox.BottomRight());
        return Vector2.Distance(nearest, Projectile.Center) <= Projectile.width / 2f;
    }

    public override void AI()
    {
        if (Age == 0)
            SoundEngine.PlaySound(SoundID.Item74 with { Pitch = -0.3f }, Projectile.Center);
        float size = SealRules.FireballSize(Age);
        Vector2 centre = Projectile.Center;
        Projectile.Resize((int)size, (int)size);
        Projectile.Center = centre;
        Projectile.rotation = Projectile.velocity.ToRotation();
        Lighting.AddLight(Projectile.Center, 2.2f * size / SealRules.FireballFullPx + 0.4f, 1.1f * size / SealRules.FireballFullPx + 0.2f, 0.25f);
        if (!Main.dedServ)
        {
            // Embers thrown off its back, and heat shimmering round it.
            for (int i = 0; i < 4; i++)
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(size / 2f, size / 2f) * Main.rand.NextFloat(0.7f, 1f),
                    DustID.Torch, -Projectile.velocity * 0.6f + Main.rand.NextVector2Circular(2f, 2f), 0, default, 2.2f).noGravity = true;
            if (Main.rand.NextBool(2))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(size / 2f, size / 2f), DustID.Smoke,
                    -Projectile.velocity * 0.3f + new Vector2(0f, -1f), 120, new Color(80, 50, 40), 2f).noGravity = true;
        }
        if (Projectile.owner == Main.myPlayer && Age % 8 == 0)
            LightGround(size);
    }

    // The ground under it catches fire, a patch every few tiles.
    private void LightGround(float size)
    {
        int x = (int)(Projectile.Center.X / 16f);
        int fromY = (int)((Projectile.Center.Y) / 16f);
        for (int y = fromY; y < fromY + (int)(size / 16f) + 10; y++)
        {
            if (!WorldGen.InWorld(x, y, 2))
                return;
            if (!WorldGen.SolidTile(x, y) || WorldGen.SolidTile(x, y - 1))
                continue;
            float groundX = x * 16f + 8f;
            if (!float.IsNaN(lastFireX) && Math.Abs(groundX - lastFireX) < 48f)
                return;
            lastFireX = groundX;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(groundX, y * 16f - 20f), Vector2.Zero,
                ModContent.ProjectileType<GroundFire>(), Math.Max(1, (int)(Projectile.damage * SealRules.GroundFireDamageShare)), 0f,
                Projectile.owner);
            return;
        }
    }

    // Against a boss or a big enemy it bursts there and then; small fry it only scorches.
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire3, 240);
        if (burstAt == null && SealRules.FireballBurstsOn(target.boss, target.lifeMax, Projectile.damage))
        {
            burstAt = target.Center;
            Projectile.Kill();
        }
    }

    public override void OnKill(int timeLeft)
    {
        Vector2 at = burstAt ?? Projectile.Center;
        SoundEngine.PlaySound(SoundID.Item62 with { Volume = 1.2f }, at);
        if (!Main.dedServ)
            Main.instance.CameraModifiers.Add(new Terraria.Graphics.CameraModifiers.PunchCameraModifier(at,
                Main.rand.NextVector2Unit(), 14f, 10f, 30, 1200f, "GreatFireball"));
        if (Projectile.owner == Main.myPlayer)
            Projectile.NewProjectile(Projectile.GetSource_Death(), at, Vector2.Zero, ModContent.ProjectileType<FireBurst>(),
                (int)(Projectile.damage * SealRules.FireballBurstDamageShare), 8f, Projectile.owner);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        float size = Projectile.width;
        bool left = Projectile.velocity.X < 0f;
        float angle = left ? Projectile.rotation - MathHelper.Pi : Projectile.rotation;
        int facing = left ? -1 : 1;
        Texture2D art = FxArt.Frame("FxHugeFireball", (int)(Main.GameUpdateCount / 4), 4)
                        ?? FxArt.Frame("FxGreatFireball", (int)(Main.GameUpdateCount / 4), 4);
        if (art != null)
        {
            float scale = size / art.Width * 1.15f;
            // The ball sits a little ahead of the art's middle (its tail of flame trails behind): set it on the hitbox.
            Vector2 at = Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 11f * scale;
            // A glow behind, then the ball.
            FxArt.Draw(art, at, new Color(255, 160, 60, 0) * 0.5f, angle, scale * 1.25f, facing);
            FxArt.Draw(art, at, Color.White, angle, scale, facing);
            return false;
        }
        Texture2D t = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
        Main.EntitySpriteDraw(t, Projectile.Center - Main.screenPosition, null, new Color(255, 170, 60, 0), Projectile.rotation,
            t.Size() / 2f, size / t.Width * 1.4f, SpriteEffects.None);
        return false;
    }
}

// Where the great fireball ends: a great burst of flame, wider than the ball.
public sealed class FireBurst : ModProjectile
{
    private const int Life = 24;
    private const int Size = 300;

    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    public override void SetDefaults()
    {
        Projectile.width = Size;
        Projectile.height = Size;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Generic;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.timeLeft = Life;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        Vector2 nearest = Vector2.Clamp(Projectile.Center, targetHitbox.TopLeft(), targetHitbox.BottomRight());
        return Projectile.timeLeft > Life - 8 && Vector2.Distance(nearest, Projectile.Center) <= Size / 2f;
    }

    public override void AI()
    {
        if (Projectile.localAI[0]++ == 0f && !Main.dedServ)
            for (int i = 0; i < 80; i++)
                Dust.NewDustPerfect(Projectile.Center, i % 3 == 0 ? DustID.Smoke : DustID.Torch, Main.rand.NextVector2Circular(12f, 12f),
                    i % 3 == 0 ? 120 : 0, default, 2.8f).noGravity = true;
        Lighting.AddLight(Projectile.Center, 3f, 1.6f, 0.4f);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire3, 300);

    public override bool PreDraw(ref Color lightColor)
    {
        int age = Life - Projectile.timeLeft;
        if (FxArt.Frame("FxHugeExplosion", Math.Min(5, age * 6 / Life), 6) is { } huge)
            FxArt.Draw(huge, Projectile.Center, Color.White, 0f, Size / (float)huge.Width * 1.2f);
        else if (FxArt.Frame("FxFireBurst", Math.Min(3, age * 4 / Life), 4) is { } art)
            FxArt.Draw(art, Projectile.Center, Color.White, 0f, Size / (float)art.Width * 1.2f);
        return false;
    }
}

// Flames left burning on the ground where the great fireball passed.
public sealed class GroundFire : ModProjectile
{
    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    public override void SetDefaults()
    {
        Projectile.width = 56;
        Projectile.height = 40;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Generic;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.timeLeft = SealRules.GroundFireTicks;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
    }

    public override void AI()
    {
        Lighting.AddLight(Projectile.Center, 0.9f, 0.45f, 0.1f);
        if (!Main.dedServ && !FxArt.Has("FxGroundFire_0") && Main.rand.NextBool(2))
            Dust.NewDustPerfect(new Vector2(Main.rand.NextFloat(Projectile.Left.X, Projectile.Right.X), Projectile.Bottom.Y - 4f), DustID.Torch,
                new Vector2(0f, -2.5f), 0, default, 1.8f).noGravity = true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire, 180);

    public override bool PreDraw(ref Color lightColor)
    {
        if (FxArt.Frame("FxGroundFire", (int)(Main.GameUpdateCount / 5 + Projectile.whoAmI), 4) is { } art)
        {
            float fade = Math.Min(1f, Projectile.timeLeft / 30f);
            FxArt.Draw(art, Projectile.Bottom - new Vector2(0f, art.Height / 2f), Color.White * fade, 0f, 1f);
        }
        return false;
    }
}

// 千鸟 (SealRules): a second and a half of lightning gathering in the hand, a charge bar over the player's head, the
// player creeping on and the crackle striking all round (user, 2026-10-03: as in the story; open to hits, which logs
// take); then a forty-tile charge towards the cursor, untouchable, stopped by a wall, through every small enemy and into
// the first boss (a far heavier blow and a burst of lightning), lightning chakra left on the path for three seconds.
// The velocity holds the direction (kept on the cursor while it gathers). The charge sounds the user's Chidori clip,
// which stays on their machine (Assets/LocalSounds, never committed); without it a vanilla sound stands in.
// Art: seal-jutsu-v2 (FxChidoriCharge, FxChidoriImpact, FxLightningTrail) and v1 (FxChidori, FxChidoriTrail).
public sealed class ChidoriCharge : ModProjectile
{
    private const string DashSound = "ShinobiPrototype/Assets/LocalSounds/ChidoriDash";

    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    private int Age => (int)Projectile.localAI[0];
    private bool Gathering => Age < SealRules.ChidoriWindupTicks;
    private Vector2 Direction => Projectile.velocity.SafeNormalize(Vector2.UnitX);
    private Vector2 lastTrail;
    private bool hitBoss;

    // How far the local player's Chidori has gathered (0 to 1), or null if none is gathering: the bar overhead.
    public static float? GatherProgress(Player player)
    {
        int type = ModContent.ProjectileType<ChidoriCharge>();
        foreach (Projectile p in Main.ActiveProjectiles)
            if (p.owner == player.whoAmI && p.type == type && p.ModProjectile is ChidoriCharge c && c.Gathering)
                return c.Age / (float)SealRules.ChidoriWindupTicks;
        return null;
    }

    public override void SetDefaults()
    {
        Projectile.width = 48;
        Projectile.height = 48;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Generic;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = SealRules.ChidoriGatherHitTicks;
        Projectile.timeLeft = SealRules.ChidoriWindupTicks + SealRules.ChidoriChargeTicks;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
    }

    // While it gathers, the crackle reaches all round the player; in the charge, the hitbox at the hand.
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        if (!Gathering)
            return null;
        Player owner = Main.player[Projectile.owner];
        Vector2 nearest = Vector2.Clamp(owner.Center, targetHitbox.TopLeft(), targetHitbox.BottomRight());
        return Vector2.Distance(nearest, owner.Center) <= SealRules.ChidoriGatherRadiusPx;
    }

    public override void AI()
    {
        Player owner = Main.player[Projectile.owner];
        if (!owner.active || owner.dead)
        {
            Projectile.Kill();
            return;
        }
        Projectile.localAI[0]++;

        if (Gathering)
        {
            // Creeping on, hands full, the lightning following the cursor.
            owner.GetModPlayer<Common.Players.SealPlayer>().ChargingTicks = 2;
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 aim = (Main.MouseWorld - owner.Center).SafeNormalize(Vector2.UnitX);
                if (Vector2.Dot(aim, Direction) < 0.999f)
                {
                    Projectile.velocity = aim;
                    if (Age % 6 == 0)
                        Projectile.netUpdate = true;
                }
            }
            Vector2 dir = Direction;
            owner.direction = dir.X >= 0f ? 1 : -1;
            Projectile.Center = owner.Center + dir * 18f;
            Lighting.AddLight(owner.Center, 0.6f + Age * 0.01f, 0.9f + Age * 0.01f, 1.6f + Age * 0.01f);
            // The chirping, rising as it gathers.
            if (Age % 6 == 1)
                SoundEngine.PlaySound(SoundID.Item93 with { Pitch = Math.Min(1f, -0.2f + Age * 0.012f), Volume = 0.65f }, owner.Center);
            if (!Main.dedServ && Main.rand.NextBool(2))
                Dust.NewDustPerfect(owner.Center + Main.rand.NextVector2CircularEdge(SealRules.ChidoriGatherRadiusPx, SealRules.ChidoriGatherRadiusPx)
                    * Main.rand.NextFloat(0.3f, 1f), DustID.Electric, Vector2.Zero, 0, default, 0.8f).noGravity = true;
            if (Age == SealRules.ChidoriWindupTicks - 1)
                StartCharge(owner);
            return;
        }

        owner.immune = true;
        owner.immuneNoBlink = true;
        owner.immuneTime = Math.Max(owner.immuneTime, 6);
        owner.fallStart = (int)(owner.position.Y / 16f);
        Vector2 direction = Direction;
        owner.direction = direction.X >= 0f ? 1 : -1;
        // The player is carried along directly (not by velocity, which the tiles would stop): through open air, and
        // through a wall up to three tiles thick to the first open spot beyond it; a thicker wall ends it.
        if (Projectile.owner == Main.myPlayer)
        {
            Vector2 next = owner.position + direction * SealRules.ChidoriSpeed;
            if (Collision.SolidCollision(next, owner.width, owner.height) && !ThroughWall(owner, direction, out next))
            {
                Projectile.Kill();
                return;
            }
            owner.position = next;
            owner.velocity = Vector2.Zero;
        }
        Projectile.Center = owner.Center + direction * 24f;
        Lighting.AddLight(Projectile.Center, 0.8f, 1.2f, 2f);
        // Lightning chakra left along the way, a piece every half its length.
        if (Projectile.owner == Main.myPlayer && Vector2.Distance(lastTrail, owner.Center) >= 32f)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), (lastTrail + owner.Center) / 2f, direction, ModContent.ProjectileType<LightningTrail>(),
                Math.Max(1, (int)(Projectile.damage * SealRules.LightningTrailDamageShare)), 0f, Projectile.owner);
            lastTrail = owner.Center;
        }
        if (!Main.dedServ && !FxArt.Has("FxChidori_0"))
            for (int i = 0; i < 4; i++)
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(14f, 14f), DustID.Electric,
                    Main.rand.NextVector2Circular(3f, 3f), 0, default, 1.1f).noGravity = true;
    }

    // The first open spot past a wall ahead, within the thickness it can go through.
    private static bool ThroughWall(Player owner, Vector2 direction, out Vector2 beyond)
    {
        float reach = SealRules.ChidoriSpeed + SealRules.ChidoriWallTiles * 16f + Math.Max(owner.width, owner.height);
        for (float d = SealRules.ChidoriSpeed + 4f; d <= reach; d += 4f)
        {
            Vector2 spot = owner.position + direction * d;
            if (!Collision.SolidCollision(spot, owner.width, owner.height))
            {
                beyond = spot;
                return true;
            }
        }
        beyond = owner.position;
        return false;
    }

    // Gathered: off it goes, with the user's Chidori sound. Enemies the crackle struck can be struck again by the charge.
    private void StartCharge(Player owner)
    {
        SoundEngine.PlaySound(ModContent.HasAsset(DashSound) ? new SoundStyle(DashSound) { Volume = 0.9f } : SoundID.Item94, owner.Center);
        lastTrail = owner.Center;
        for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
            Projectile.localNPCImmunity[i] = 0;
        Projectile.localNPCHitCooldown = -1;
    }

    // The crackle while it gathers is a fraction of the blow; into a boss the charge is far heavier, and stops there.
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Gathering)
            modifiers.SourceDamage *= SealRules.ChidoriGatherDamageShare;
        else if (target.boss)
            modifiers.SourceDamage *= SealRules.ChidoriBossMultiplier;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Electrified, Gathering ? 60 : 180);
        if (Gathering)
            return;
        SoundEngine.PlaySound(SoundID.Item122 with { Volume = 0.7f }, target.Center);
        if (target.boss && !hitBoss)
        {
            hitBoss = true;
            if (!Main.dedServ)
                Main.instance.CameraModifiers.Add(new Terraria.Graphics.CameraModifiers.PunchCameraModifier(target.Center,
                    Direction, 12f, 12f, 20, 1000f, "Chidori"));
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<ChidoriImpact>(),
                0, 0f, Projectile.owner);
            Projectile.Kill();
        }
    }

    public override void OnKill(int timeLeft)
    {
        Player owner = Main.player[Projectile.owner];
        // A little of the charge carries on.
        if (!Gathering && Projectile.owner == Main.myPlayer)
            owner.velocity = Direction * 4f;
        owner.immuneNoBlink = false;
        if (!Main.dedServ)
            for (int i = 0; i < 30; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.Electric, Main.rand.NextVector2Circular(6f, 6f), 0, default, 1.4f).noGravity = true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Vector2 dir = Direction;
        if (Gathering)
        {
            Texture2D gather = FxArt.Frame("FxChidoriCharge", (int)(Main.GameUpdateCount / 2), 4) ?? FxArt.Frame("FxChidori", (int)(Main.GameUpdateCount / 2), 4);
            if (gather != null)
            {
                float grow = 0.5f + 0.9f * Age / SealRules.ChidoriWindupTicks;
                FxArt.Draw(gather, Projectile.Center, new Color(150, 200, 255, 0) * 0.6f, Main.rand.NextFloat(-0.3f, 0.3f), grow * 1.4f);
                FxArt.Draw(gather, Projectile.Center, Color.White, 0f, grow);
            }
            return false;
        }
        if (FxArt.Frame("FxChidoriTrail", Age / 3, 3) is { } trail)
            FxArt.Draw(trail, Projectile.Center - dir * 60f, Color.White, dir.ToRotation(), 2f);
        Texture2D hand = FxArt.Frame("FxChidoriCharge", (int)(Main.GameUpdateCount / 2), 4) ?? FxArt.Frame("FxChidori", (int)(Main.GameUpdateCount / 3), 4);
        if (hand != null)
            FxArt.Draw(hand, Projectile.Center, Color.White, 0f, 1.2f);
        return false;
    }
}

// Lightning chakra left on Chidori's path: a short crackling stretch (ai along the path) that strikes whatever stands in
// it every quarter second for three seconds.
public sealed class LightningTrail : ModProjectile
{
    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    public override void SetDefaults()
    {
        Projectile.width = 40;
        Projectile.height = 40;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Generic;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = SealRules.LightningTrailHitTicks;
        Projectile.timeLeft = SealRules.LightningTrailTicks;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
    }

    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.position -= Projectile.velocity;   // stays where it was left
        Lighting.AddLight(Projectile.Center, 0.3f, 0.5f, 1f);
        if (!Main.dedServ && !FxArt.Has("FxLightningTrail_0") && Main.rand.NextBool(3))
            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16f, 8f), DustID.Electric, Vector2.Zero, 0, default, 0.9f)
                .noGravity = true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Electrified, 60);

    public override bool PreDraw(ref Color lightColor)
    {
        if (FxArt.Frame("FxLightningTrail", (int)(Main.GameUpdateCount / 3 + Projectile.whoAmI), 4) is { } art)
        {
            float fade = Math.Min(1f, Projectile.timeLeft / 40f);
            FxArt.Draw(art, Projectile.Center, Color.White * fade, Projectile.rotation, 1f);
        }
        return false;
    }
}

// The burst of lightning where Chidori drives into a boss.
public sealed class ChidoriImpact : ModProjectile
{
    private const int Life = 20;

    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = Life;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
    }

    public override void AI()
    {
        Lighting.AddLight(Projectile.Center, 1.5f, 2f, 3f);
        if (Projectile.localAI[0]++ == 0f && !Main.dedServ)
            for (int i = 0; i < 50; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.Electric, Main.rand.NextVector2Circular(10f, 10f), 0, default, 1.6f).noGravity = true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        int age = Life - Projectile.timeLeft;
        if (FxArt.Frame("FxChidoriImpact", Math.Min(4, age * 5 / Life), 5) is { } art)
            FxArt.Draw(art, Projectile.Center, Color.White, 0f, 1.2f);
        return false;
    }
}
