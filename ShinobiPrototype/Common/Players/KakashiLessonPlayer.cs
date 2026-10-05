using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items.Jutsu;
using ShinobiPrototype.Content.Items.Weapons;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Common.Players;

// Kakashi's three lessons between Tazuna's confession and the lake (specs/空档衔接与火影小兵.spec.md, gap A; user
// 2026-10-04): find Pakkun (the ninken summoning), the Great Fireball at a lake, and tree climbing (running up walls
// on chakra, WallWalkPlayer). In order, one at a time; a character already past that point can still take them.
public enum KakashiLesson : byte { Pakkun, Fireball, Climbing, Done }

public sealed class KakashiLessonPlayer : ModPlayer
{
    public const int PakkunMinTiles = 100;
    public const int PakkunMaxTiles = 200;
    public const float PakkunFoundTiles = 3f;
    public const int FireballAttempts = 3;
    public const int ClimbTiles = 15;

    public KakashiLesson Next { get; private set; }
    public bool Active { get; private set; }
    private int fireballAttempts;
    private int lostPakkun = -1;

    public bool Learned(KakashiLesson lesson) => Next > lesson;

    // Development shortcut (/m0 lesson): make this the next lesson, not yet started.
    public void SetForTesting(KakashiLesson next)
    {
        Next = next;
        Active = false;
        fireballAttempts = 0;
    }

    // Open once Tazuna has come clean about the mission (or the Land of Waves is already behind the player).
    public static bool Open => StoryWorld.TazunaConfessed || StoryWorld.WaveComplete;

    public override void Initialize()
    {
        Next = KakashiLesson.Pakkun;
        Active = false;
        fireballAttempts = 0;
        lostPakkun = -1;
    }

    // Kakashi's "lesson" button: start the next lesson, or repeat what it asks.
    public string Ask()
    {
        if (Next == KakashiLesson.Done)
            return Loc.Get("Lesson.AllDone");
        if (!Active)
        {
            Active = true;
            fireballAttempts = 0;
            if (Next == KakashiLesson.Pakkun)
                LosePakkun();
            return Loc.Get($"Lesson.{Next}.Start", ShinobiKeybinds.SealKeyName(4));
        }
        if (Next == KakashiLesson.Pakkun && (lostPakkun < 0 || !Main.projectile[lostPakkun].active))
            LosePakkun();
        return Loc.Get($"Lesson.{Next}.Remind", ShinobiKeybinds.SealKeyName(4));
    }

    // Pakkun waits somewhere on the surface 100 to 200 tiles away, only for this player.
    private void LosePakkun()
    {
        if (Player.whoAmI != Main.myPlayer)
            return;
        for (int tries = 0; tries < 40; tries++)
        {
            int x = (int)(Player.Center.X / 16f) + (Main.rand.NextBool() ? 1 : -1) * Main.rand.Next(PakkunMinTiles, PakkunMaxTiles + 1);
            if (!WorldGen.InWorld(x, 50, 50))
                continue;
            for (int y = 50; y < Main.worldSurface; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile || !Main.tileSolid[tile.TileType] || tile.LiquidAmount > 0)
                    continue;
                if (Collision.SolidCollision(new Vector2(x * 16f - 14f, y * 16f - 24f), 28, 22))
                    break;
                Vector2 at = new(x * 16f + 8f, y * 16f - 11f);
                lostPakkun = Projectile.NewProjectile(Player.GetSource_Misc("KakashiLesson"), at, Vector2.Zero,
                    ModContent.ProjectileType<LostPakkun>(), 0, 0f, Player.whoAmI);
                Main.NewText(Loc.Get("Lesson.Pakkun.Direction", Loc.Get(StoryRules.Direction(x - (int)(Player.Center.X / 16f)))),
                    200, 210, 230);
                return;
            }
        }
    }

    public override void PostUpdate()
    {
        if (Player.whoAmI != Main.myPlayer || !Active)
            return;
        if (Next == KakashiLesson.Pakkun && lostPakkun >= 0 && Main.projectile[lostPakkun] is { active: true } pakkun &&
            pakkun.ModProjectile is LostPakkun && pakkun.Distance(Player.Center) < PakkunFoundTiles * 16f)
        {
            pakkun.Kill();
            lostPakkun = -1;
            Finish(ModContent.ItemType<NinkenScroll>());
        }
    }

    // The fireball lesson takes the 4-seal key at a lake (SealPlayer asks first). True when the press was a practice.
    public bool TryFireballPractice()
    {
        if (!Active || Next != KakashiLesson.Fireball || LakeAmbushSystem.FindLakeNear(Player) == null)
            return false;
        fireballAttempts++;
        float radius = 16f + 30f * fireballAttempts;
        Vector2 at = Player.Center + new Vector2(Player.direction * (40f + radius), -8f);
        SoundEngine.PlaySound(fireballAttempts < FireballAttempts ? SoundID.Item20 : SoundID.Item74, at);
        for (int i = 0; i < 20 + 25 * fireballAttempts; i++)
            Dust.NewDustPerfect(at + Main.rand.NextVector2Circular(radius, radius), DustID.Torch,
                new Vector2(Player.direction * Main.rand.NextFloat(1f, 4f), Main.rand.NextFloat(-1.5f, 0.5f)), 0, default, 1.8f).noGravity = true;
        CombatText.NewText(Player.getRect(), new Color(255, 170, 90), Loc.Get($"Lesson.Fireball.Try{System.Math.Min(fireballAttempts, FireballAttempts)}"));
        if (fireballAttempts >= FireballAttempts)
            Finish(ModContent.ItemType<ScrollFireball>());
        return true;
    }

    // WallWalkPlayer reports a climb up a wall.
    public void Climbed(int tiles)
    {
        if (Active && Next == KakashiLesson.Climbing && tiles >= ClimbTiles)
            Finish(0);
    }

    private void Finish(int reward)
    {
        Main.NewText(Loc.Get($"Lesson.{Next}.Done"), 200, 210, 230);
        if (reward > 0)
            Player.QuickSpawnItem(Player.GetSource_Misc("KakashiLesson"), reward);
        Next++;
        Active = false;
        SoundEngine.PlaySound(SoundID.Item4, Player.Center);
    }

    public override void SaveData(TagCompound tag)
    {
        if (Next != KakashiLesson.Pakkun)
            tag["kakashiLesson"] = (byte)Next;
    }

    public override void LoadData(TagCompound tag) =>
        Next = (KakashiLesson)System.Math.Min((byte)KakashiLesson.Done, tag.GetByte("kakashiLesson"));
}

// Tree climbing (Kakashi's third lesson): against a wall in the air, the player clings and slides like with climbing
// claws (vanilla's spiked boots), may jump off it, and holding up or toward the wall runs up it for 5 chakra a second;
// out of chakra they slide down. Learned for good once the lesson is done; during it, it works for the lesson.
public sealed class WallWalkPlayer : ModPlayer
{
    public const float ChakraPerSecond = 5f;
    public const float ClimbSpeed = 2.8f;

    private float climbStartY = -1f;

    private bool CanClimb
    {
        get
        {
            KakashiLessonPlayer lessons = Player.GetModPlayer<KakashiLessonPlayer>();
            return lessons.Learned(KakashiLesson.Climbing) || lessons.Active && lessons.Next == KakashiLesson.Climbing;
        }
    }

    public override void PostUpdateEquips()
    {
        if (CanClimb)
            Player.spikedBoots = System.Math.Max(Player.spikedBoots, 2);
    }

    public override void PostUpdate()
    {
        if (Player.whoAmI != Main.myPlayer || !CanClimb || !Player.sliding)
        {
            climbStartY = -1f;
            return;
        }
        bool toward = Player.slideDir > 0 ? Player.controlRight : Player.controlLeft;
        if (!(Player.controlUp || toward) || !Player.GetModPlayer<ChakraPlayer>().TryDrain(ChakraPerSecond / 60f))
        {
            climbStartY = -1f;
            return;
        }
        if (climbStartY < 0f)
            climbStartY = Player.position.Y;
        Player.velocity.Y = -ClimbSpeed;
        Player.fallStart = (int)(Player.position.Y / 16f);
        if (Main.rand.NextBool(4))
            Dust.NewDust(Player.position, Player.width, Player.height, DustID.MagicMirror, 0f, 0f, 150, default, 0.8f);
        Player.GetModPlayer<KakashiLessonPlayer>().Climbed((int)((climbStartY - Player.position.Y) / 16f));
    }
}
