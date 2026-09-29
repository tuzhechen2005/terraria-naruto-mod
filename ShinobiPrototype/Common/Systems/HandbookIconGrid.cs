using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace ShinobiPrototype.Common.Systems;

// A row of icons for the handbook's reward and path pages: each drawn in colour when unlocked or as a solid black
// silhouette when not, with its name underneath and optional tier boxes.
internal sealed class HandbookIconGrid : UIElement
{
    public readonly record struct Entry(Texture2D Texture, string Name, bool Unlocked, int Tiers = 0);

    private const float Cell = 84f;
    private const float Icon = 56f;
    private readonly List<Entry> entries = new();

    public void Set(IEnumerable<Entry> items)
    {
        entries.Clear();
        entries.AddRange(items);
    }

    public bool IsEmpty => entries.Count == 0;

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        CalculatedStyle area = GetDimensions();
        float startX = area.X + (area.Width - entries.Count * Cell) / 2f;
        for (int i = 0; i < entries.Count; i++)
        {
            Entry entry = entries[i];
            Vector2 center = new(startX + i * Cell + Cell / 2f, area.Y + Icon / 2f + 4f);
            if (entry.Texture != null)
            {
                float scale = Icon / System.Math.Max(entry.Texture.Width, entry.Texture.Height);
                spriteBatch.Draw(entry.Texture, center, null, entry.Unlocked ? Color.White : Color.Black, 0f,
                    entry.Texture.Size() / 2f, scale, SpriteEffects.None, 0f);
            }
            string name = entry.Unlocked || entry.Tiers > 0 ? entry.Name : "？？？";
            Vector2 size = FontAssets.MouseText.Value.MeasureString(name) * 0.7f;
            Utils.DrawBorderString(spriteBatch, name, new Vector2(center.X - size.X / 2f, center.Y + Icon / 2f + 2f),
                entry.Unlocked ? Color.White : Color.Gray, 0.7f);
            for (int t = 0; t < entry.Tiers; t++)
            {
                Rectangle box = new((int)(center.X - 27 + t * 19), (int)(center.Y + Icon / 2f + 22f), 16, 16);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, box, Color.Black * 0.6f);
                Utils.DrawBorderString(spriteBatch, "?", new Vector2(box.X + 4, box.Y - 1), Color.Gray, 0.7f);
            }
        }
    }
}
