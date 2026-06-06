using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using TheMarauderMap.Data;
using TheMarauderMap.Projection;

namespace TheMarauderMap.Rendering;

public sealed class PredictionRenderer : IDisposable
{
    private readonly MapProjectionService _projection;
    private Texture2D? _markerTexture;
    private Texture2D? _pixelTexture;

    public PredictionRenderer(MapProjectionService projection)
    {
        _projection = projection;
    }

    public void Draw(
        SpriteBatch spriteBatch,
        IReadOnlyDictionary<string, IReadOnlyList<PredictionPoint>> predictions,
        Vector2 viewportOffset,
        float zoom)
    {
        Draw(spriteBatch, predictions, new MapScreenTransform(viewportOffset, zoom, null));
    }

    public void Draw(
        SpriteBatch spriteBatch,
        IReadOnlyDictionary<string, IReadOnlyList<PredictionPoint>> predictions,
        MapScreenTransform transform)
    {
        _markerTexture ??= CreateMarkerTexture(Game1.graphics.GraphicsDevice);

        foreach ((string npcName, IReadOnlyList<PredictionPoint> points) in predictions)
        {
            DrawPredictionPath(spriteBatch, npcName, points, transform);
        }
    }

    public void Draw(
        SpriteBatch spriteBatch,
        IReadOnlyDictionary<string, IReadOnlyList<PredictionPoint>> predictions,
        MapContentTransform transform)
    {
        _markerTexture ??= CreateMarkerTexture(Game1.graphics.GraphicsDevice);

        foreach ((string npcName, IReadOnlyList<PredictionPoint> points) in predictions)
        {
            DrawPredictionPath(spriteBatch, npcName, points, transform);
        }
    }

    private void DrawPredictionPath(
        SpriteBatch spriteBatch,
        string npcName,
        IReadOnlyList<PredictionPoint> points,
        MapScreenTransform transform)
    {
        DrawPredictionPath(spriteBatch, npcName, points, transform.MapToScreen, transform.Zoom);
    }

    private void DrawPredictionPath(
        SpriteBatch spriteBatch,
        string npcName,
        IReadOnlyList<PredictionPoint> points,
        MapContentTransform transform)
    {
        DrawPredictionPath(spriteBatch, npcName, points, transform.MapToScreen, transform.Zoom);
    }

    private void DrawPredictionPath(
        SpriteBatch spriteBatch,
        string npcName,
        IReadOnlyList<PredictionPoint> points,
        Func<Vector2, Vector2> mapToScreen,
        float zoom)
    {
        if (points.Count == 0)
            return;

        Vector2? previousMapPosition = null;

        for (int i = 0; i < points.Count; i++)
        {
            PredictionPoint point = points[i];
            if (!_projection.TryProject(point.LocationName, point.TilePosition, out Vector2 mapPosition))
                continue;

            Vector2 screenPosition = mapToScreen(mapPosition);

            float alpha = MathHelper.Lerp(0.7f, 0.2f, (float)i / Math.Max(1, points.Count - 1));
            Color markerColor = Color.CornflowerBlue * alpha;
            float scale = MathHelper.Clamp(zoom, 0.6f, 1.2f);

            if (_markerTexture is null) continue;
            spriteBatch.Draw(_markerTexture, screenPosition, null, markerColor, 0f,
                new Vector2(_markerTexture.Width / 2f, _markerTexture.Height / 2f),
                scale, SpriteEffects.None, 0f);

            string timeLabel = FormatTime(point.TimeOfDay);
            Vector2 labelSize = Game1.smallFont.MeasureString(timeLabel) * scale * 0.8f;
            Vector2 labelPosition = screenPosition + new Vector2(-labelSize.X / 2f, 10 * scale);

            Color labelColor = Color.CornflowerBlue * (alpha * 0.8f);
            spriteBatch.DrawString(Game1.smallFont, timeLabel, labelPosition, labelColor, 0f,
                Vector2.Zero, scale * 0.8f, SpriteEffects.None, 0f);

            if (previousMapPosition.HasValue)
            {
                DrawLine(spriteBatch,
                    mapToScreen(previousMapPosition.Value),
                    screenPosition,
                    Color.CornflowerBlue * MathHelper.Clamp(alpha * 0.9f, 0.35f, 0.75f),
                    zoom);
            }

            previousMapPosition = mapPosition;
        }
    }

    private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float zoom)
    {
        _pixelTexture ??= CreatePixelTexture(Game1.graphics.GraphicsDevice);

        Vector2 delta = end - start;
        float length = delta.Length();
        if (length < 8f)
            return;

        float rotation = MathF.Atan2(delta.Y, delta.X);
        float width = GetPathLineWidth(zoom);
        spriteBatch.Draw(_pixelTexture, start, null, color, rotation, new Vector2(0f, 0.5f), new Vector2(length, width), SpriteEffects.None, 0f);
    }


    private static string FormatTime(int timeOfDay)
    {
        int hours = timeOfDay / 100;
        int minutes = timeOfDay % 100;
        return $"{hours:D2}:{minutes:D2}";
    }

    public void Dispose()
    {
        _markerTexture?.Dispose();
        _markerTexture = null;
        _pixelTexture?.Dispose();
        _pixelTexture = null;
    }

    private static Texture2D CreateMarkerTexture(GraphicsDevice graphicsDevice)
    {
        const int size = 6;
        var texture = new Texture2D(graphicsDevice, size, size);
        var data = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distFromCenter = Vector2.Distance(new Vector2(x, y), new Vector2(size / 2f, size / 2f));
                data[y * size + x] = distFromCenter <= size / 2f ? Color.White : Color.Transparent;
            }
        }

        texture.SetData(data);
        return texture;
    }

    private static Texture2D CreatePixelTexture(GraphicsDevice graphicsDevice)
    {
        var texture = new Texture2D(graphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        return texture;
    }

    public static float GetPathLineWidth(float zoom)
    {
        return MathHelper.Clamp(4f * zoom, 3f, 6f);
    }
}
