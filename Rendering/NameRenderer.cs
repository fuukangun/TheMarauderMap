using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace TheMarauderMap.Rendering;

public sealed class NameRenderer
{
    private const string HeartAssetPath = "assets/heart.png";

    private readonly string _modDirectory;
    private Texture2D? _heartTexture;

    private static readonly Vector2[] OutlineOffsets =
    {
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1),
        new(-1, -1), new(1, -1), new(-1, 1), new(1, 1)
    };

    public NameRenderer(string modDirectory)
    {
        _modDirectory = modDirectory;
    }

    public void DrawName(SpriteBatch spriteBatch, string displayName, Vector2 screenPosition, Color color, float zoom, bool isSelected = false, bool showHeart = false)
    {
        float scale = MathHelper.Clamp(zoom, 0.8f, 1.25f);
        Vector2 size = Game1.smallFont.MeasureString(displayName) * scale;
        Vector2 origin = size / 2f / scale;

        if (isSelected)
            DrawSelectionHighlight(spriteBatch, displayName, screenPosition, origin, scale);

        DrawOutlinedText(spriteBatch, displayName, screenPosition, color, origin, scale);

        if (showHeart)
            DrawHeart(spriteBatch, screenPosition, size, scale);
    }

    private void DrawHeart(SpriteBatch spriteBatch, Vector2 textPosition, Vector2 textSize, float scale)
    {
        _heartTexture ??= LoadHeartTexture(spriteBatch.GraphicsDevice, _modDirectory);
        HeartLayout layout = CalculateHeartLayout(textPosition, textSize);
        Vector2 origin = new(_heartTexture.Width / 2f, _heartTexture.Height / 2f);
        float heartScale = layout.Size / _heartTexture.Height;

        spriteBatch.Draw(_heartTexture, layout.Position, null, Color.White, 0f, origin, heartScale, SpriteEffects.None, 0f);
    }

    public static HeartLayout CalculateHeartLayout(Vector2 textPosition, Vector2 textSize)
    {
        float size = textSize.Y;
        Vector2 position = new(
            textPosition.X + textSize.X / 2f + size / 2f + 4f,
            textPosition.Y);

        return new HeartLayout(position, size);
    }

    public static string GetHeartAssetPath(string modDirectory)
    {
        return Path.Combine(modDirectory, HeartAssetPath);
    }

    private static void DrawSelectionHighlight(SpriteBatch spriteBatch, string text, Vector2 position, Vector2 origin, float scale)
    {
        Vector2 size = Game1.smallFont.MeasureString(text) * scale;
        Rectangle highlightRect = new(
            (int)(position.X - size.X / 2f - 4),
            (int)(position.Y - size.Y / 2f - 2),
            (int)(size.X + 8),
            (int)(size.Y + 4));

        Texture2D pixel = new(spriteBatch.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
        spriteBatch.Draw(pixel, highlightRect, Color.Gold * 0.25f);
        pixel.Dispose();
    }

    private static void DrawOutlinedText(SpriteBatch spriteBatch, string text, Vector2 position, Color color, Vector2 origin, float scale)
    {
        Color outline = Color.Black * 0.85f;

        foreach (Vector2 offset in OutlineOffsets)
            spriteBatch.DrawString(Game1.smallFont, text, position + offset, outline, 0f, origin, scale, SpriteEffects.None, 0f);

        spriteBatch.DrawString(Game1.smallFont, text, position, color, 0f, origin, scale, SpriteEffects.None, 0f);
    }

    private static Texture2D LoadHeartTexture(GraphicsDevice graphicsDevice, string modDirectory)
    {
        string assetPath = GetHeartAssetPath(modDirectory);
        if (File.Exists(assetPath))
        {
            try
            {
                using FileStream stream = File.OpenRead(assetPath);
                return Texture2D.FromStream(graphicsDevice, stream);
            }
            catch
            {
                return CreateFallbackHeartTexture(graphicsDevice);
            }
        }

        return CreateFallbackHeartTexture(graphicsDevice);
    }

    private static Texture2D CreateFallbackHeartTexture(GraphicsDevice graphicsDevice)
    {
        const int width = 15;
        const int height = 13;
        var texture = new Texture2D(graphicsDevice, width, height);
        var data = new Color[width * height];

        for (int i = 0; i < data.Length; i++)
            data[i] = Color.Transparent;

        string[] rows =
        {
            "..###...###....",
            ".#####.#####...",
            "#############..",
            "#############..",
            ".###########...",
            "..#########....",
            "...#######.....",
            "....#####......",
            ".....###.......",
            "......#........",
            "...............",
            "...............",
            "..............."
        };

        for (int y = 0; y < rows.Length; y++)
        {
            for (int x = 0; x < rows[y].Length; x++)
            {
                if (rows[y][x] == '#')
                    data[y * width + x] = Color.Red;
            }
        }

        texture.SetData(data);
        return texture;
    }
}

public readonly record struct HeartLayout(Vector2 Position, float Size);
