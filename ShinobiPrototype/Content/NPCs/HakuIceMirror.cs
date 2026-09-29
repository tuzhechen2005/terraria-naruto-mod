using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;

namespace ShinobiPrototype.Content.NPCs;

// One mirror of Haku's cage or frenzy ring. ai[0] = owner whoAmI + 1, ai[1] = slot.
// Its position is derived from the owner's synced center, so every client agrees.
public sealed class HakuIceMirror : ModNPC
{
    public override string Texture => "ShinobiPrototype/Content/NPCs/HakuIceMirrorV2";

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 1;
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers { Hide = true });
    }

    public override void SetDefaults()
    {
        NPC.width = 40;
        NPC.height = 64;
        NPC.damage = 0;
        NPC.defense = 0;
        NPC.lifeMax = WaveDuoRules.CageMirrorLife;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.aiStyle = -1;
        NPC.value = 0f;
        NPC.npcSlots = 0f;
        NPC.HitSound = SoundID.Item27;
        NPC.DeathSound = SoundID.Shatter;
    }

    private HakuBoss Owner()
    {
        int index = (int)NPC.ai[0] - 1;
        if (index < 0 || index >= Main.maxNPCs)
            return null;
        NPC owner = Main.npc[index];
        return owner.active && owner.ModNPC is HakuBoss haku ? haku : null;
    }

    public override void AI()
    {
        HakuBoss owner = Owner();
        if (owner == null || !owner.MirrorsActive)
        {
            Shatter(12);
            NPC.active = false;
            return;
        }
        NPC.Center = owner.MirrorPosition((int)NPC.ai[1]);
        NPC.velocity = Vector2.Zero;
        NPC.localAI[0] = Math.Min(1f, NPC.localAI[0] + 1f / 20f); // fade in
        Lighting.AddLight(NPC.Center, 0.2f, 0.45f, 0.6f);
        if (owner.MirrorGlowing((int)NPC.ai[1]) && Main.netMode != NetmodeID.Server &&
            Main.rand.NextBool(2))
            Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(18f, 30f),
                DustID.IceTorch, Vector2.Zero, 40, new Color(220, 250, 255), 1.3f).noGravity = true;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0)
            Shatter(22);
        else if (Main.netMode != NetmodeID.Server)
            for (int i = 0; i < 4; i++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.IceTorch);
    }

    private void Shatter(int count)
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        for (int i = 0; i < count; i++)
            Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(18f, 30f),
                DustID.IceTorch, Main.rand.NextVector2Circular(3f, 3f), 30,
                new Color(200, 245, 255), 1.3f).noGravity = true;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        HakuBoss owner = Owner();
        bool glowing = owner != null && owner.MirrorGlowing((int)NPC.ai[1]);
        float fade = NPC.localAI[0];
        float pulse = glowing ? 0.85f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 18f) * 0.15f : 0.8f;
        Color tint = (glowing ? new Color(235, 252, 255) : new Color(145, 225, 255)) * (pulse * fade);
        int broken = NPC.life < NPC.lifeMax / 2 ? 1 : 0;
        string frame = $"ShinobiPrototype/Content/NPCs/IceMirror_{broken}";
        Vector2 center = NPC.Center - screenPos;
        center = new Vector2((float)Math.Round(center.X), (float)Math.Round(center.Y));
        if (ModContent.HasAsset(frame))
        {
            Texture2D art = ModContent.Request<Texture2D>(frame).Value;
            spriteBatch.Draw(art, center, null, tint, 0f, art.Size() * 0.5f, 1f,
                SpriteEffects.None, 0f);
            return false;
        }
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        spriteBatch.Draw(texture, center, null, tint, 0f, texture.Size() * 0.5f,
            64f / texture.Height, SpriteEffects.None, 0f);
        return false;
    }

    public override bool CheckActive() => false;
}
