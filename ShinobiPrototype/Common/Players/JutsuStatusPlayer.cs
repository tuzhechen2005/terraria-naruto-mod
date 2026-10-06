using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Players;

// What the exam fights do to the player (ExamBossRules), all on the player's own client, where the hits land:
// bound in Gaara's sand coffin, frozen by Orochimaru's killing intent (substitution breaks both), ears ringing from
// Dosu's drill (left and right swap), chakra points sealed by Neji (less maximum chakra; three seals stop substitution),
// chakra recovery stopped by Orochimaru's Five Elements Seal.
public sealed class JutsuStatusPlayer : ModPlayer
{
    public int BindTicks { get; private set; }
    public int FearTicks { get; private set; }
    public int TinnitusTicks { get; private set; }
    public int Seals { get; private set; }
    public int RegenSealTicks { get; private set; }
    // The Five Elements Seal's mark shows on the body for a moment when it lands (SealGlyphLayer).
    public int SealGlyphTicks { get; private set; }
    public const int SealGlyphShowTicks = 75;

    public void ShowSealGlyph() => SealGlyphTicks = SealGlyphShowTicks;
    private int sealTicks;

    public bool Bound => ExamBossRules.BreaksOnSubstitution(BindTicks, FearTicks);
    public int SealedChakra => ExamBossRules.SealedChakra(Seals);
    public bool SubstitutionSealed => ExamBossRules.SubstitutionSealed(Seals);

    public override void Initialize()
    {
        BindTicks = FearTicks = TinnitusTicks = Seals = sealTicks = RegenSealTicks = 0;
    }

    public override void OnRespawn() => Initialize();

    public void Bind(int ticks)
    {
        BindTicks = System.Math.Max(BindTicks, ticks);
        CombatText.NewText(Player.getRect(), new Color(230, 200, 130), Loc.Get("Status.SandCoffin"));
    }

    public void Fear(int ticks)
    {
        FearTicks = System.Math.Max(FearTicks, ticks);
        CombatText.NewText(Player.getRect(), new Color(200, 60, 80), Loc.Get("Status.KillingIntent"));
    }

    public void Ring(int ticks)
    {
        TinnitusTicks = ExamBossRules.Tinnitus(TinnitusTicks, ticks);
        Main.instance.CameraModifiers.Add(new PunchCameraModifier(Player.Center, Main.rand.NextVector2Unit(), 4f, 8f, 20));
    }

    public void Seal()
    {
        Seals = ExamBossRules.AddSeal(Seals);
        sealTicks = ExamBossRules.SealTicks;
        CombatText.NewText(Player.getRect(), new Color(170, 200, 255),
            SubstitutionSealed ? Loc.Get("Status.PointsSealed") : Loc.Get("Status.PointSealed", Seals));
    }

    // Orochimaru's Five Elements Seal.
    public void SealRegen(int ticks)
    {
        RegenSealTicks = System.Math.Max(RegenSealTicks, ticks);
        CombatText.NewText(Player.getRect(), new Color(190, 150, 230), Loc.Get("Status.FiveSeal"));
    }

    // Substitution got the player out.
    public void Break()
    {
        BindTicks = 0;
        FearTicks = 0;
    }

    public override void PostUpdate()
    {
        if (BindTicks > 0)
            BindTicks--;
        if (FearTicks > 0)
            FearTicks--;
        if (TinnitusTicks > 0)
            TinnitusTicks--;
        if (RegenSealTicks > 0)
            RegenSealTicks--;
        if (SealGlyphTicks > 0)
            SealGlyphTicks--;
        if (sealTicks > 0 && --sealTicks == 0)
            Seals = 0;
    }

    public override void SetControls()
    {
        if (BindTicks > 0 || FearTicks > 0)
        {
            Player.controlLeft = Player.controlRight = Player.controlJump = Player.controlUp = Player.controlDown = false;
            Player.controlUseItem = Player.controlUseTile = Player.controlHook = Player.controlMount = false;
            if (BindTicks > 0)
                Player.velocity = new Vector2(0f, System.Math.Max(0f, Player.velocity.Y) * 0.2f);
        }
        else if (TinnitusTicks > 0)
            (Player.controlLeft, Player.controlRight) = (Player.controlRight, Player.controlLeft);
    }

    public override void PostUpdateRunSpeeds()
    {
        if (FearTicks > 0)
            Player.velocity.X *= 0.5f;
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (BindTicks > 0 && Main.rand.NextBool(2))
            Dust.NewDust(Player.position, Player.width, Player.height, Terraria.ID.DustID.Sand, 0f, -0.5f);
        if (FearTicks > 0)
        {
            r *= 1f;
            g *= 0.5f;
            b *= 0.5f;
        }
        if (RegenSealTicks > 0 && Main.rand.NextBool(4))
            Dust.NewDust(Player.position, Player.width, Player.height, Terraria.ID.DustID.PurpleTorch, 0f, -1f);
        if (Seals > 0 && Main.rand.NextBool(8))
            Dust.NewDust(Player.position, Player.width, Player.height, Terraria.ID.DustID.BlueTorch, 0f, -1f);
    }
}
