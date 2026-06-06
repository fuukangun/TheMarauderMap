using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using TheMarauderMap.Data;
using TheMarauderMap.Projection;

namespace TheMarauderMap.Rendering;

public sealed class FootprintRenderer : IDisposable
{
    private const string FootprintAssetPath = "assets/footprints.png";

    private readonly MapProjectionService _projection;
    private readonly string _modDirectory;
    private Texture2D? _footprintTexture;

    public FootprintRenderer(MapProjectionService projection, string modDirectory)
    {
        _projection = projection;
        _modDirectory = modDirectory;
    }

    public void Draw(
        SpriteBatch spriteBatch,
        IReadOnlyDictionary<string, IReadOnlyList<FootprintPoint>> footprints,
        int maxVisiblePoints,
        Vector2 viewportOffset,
        float zoom)
    {
        Draw(spriteBatch, footprints, maxVisiblePoints, null, MapDisplayContext.Mainland, new MapScreenTransform(viewportOffset, zoom, null));
    }

    public void Draw(
        SpriteBatch spriteBatch,
        IReadOnlyDictionary<string, IReadOnlyList<FootprintPoint>> footprints,
        int maxVisiblePoints,
        string? selectedNpcName,
        MapDisplayContext displayContext,
        MapScreenTransform transform)
    {
        _footprintTexture ??= CreateFootprintTexture(Game1.graphics.GraphicsDevice, _modDirectory);

        foreach (FootprintRenderPath renderPath in FootprintRenderPlanner.PlanPaths(footprints, selectedNpcName))
        {
            IReadOnlyList<FootprintPoint> path = renderPath.Points;
            for (int i = 1; i < path.Count; i++)
            {
                FootprintPoint previous = path[i - 1];
                FootprintPoint current = path[i];

                if (!displayContext.AllowsLocation(previous.LocationName) || !displayContext.AllowsLocation(current.LocationName))
                    continue;

                if (!FootprintRenderPlanner.ShouldConnectProjectedPoints(previous, current))
                    continue;

                if (!_projection.TryProject(previous.LocationName, previous.TilePosition, out Vector2 start))
                    continue;

                if (!_projection.TryProject(current.LocationName, current.TilePosition, out Vector2 end))
                    continue;

                float startAgeRatio = FootprintRenderPlanner.GetPointAgeRatio(i - 1, path.Count);
                float endAgeRatio = FootprintRenderPlanner.GetPointAgeRatio(i, path.Count);
                DrawSegment(spriteBatch, start, end, current.MovementType, startAgeRatio, endAgeRatio, transform);
            }
        }
    }

    public void Draw(
        SpriteBatch spriteBatch,
        IReadOnlyDictionary<string, IReadOnlyList<FootprintPoint>> footprints,
        int maxVisiblePoints,
        string? selectedNpcName,
        MapDisplayContext displayContext,
        MapContentTransform transform)
    {
        _footprintTexture ??= CreateFootprintTexture(Game1.graphics.GraphicsDevice, _modDirectory);

        foreach (FootprintRenderPath renderPath in FootprintRenderPlanner.PlanPaths(footprints, selectedNpcName))
        {
            IReadOnlyList<FootprintPoint> path = renderPath.Points;
            for (int i = 1; i < path.Count; i++)
            {
                FootprintPoint previous = path[i - 1];
                FootprintPoint current = path[i];

                if (!displayContext.AllowsLocation(previous.LocationName) || !displayContext.AllowsLocation(current.LocationName))
                    continue;

                if (!FootprintRenderPlanner.ShouldConnectProjectedPoints(previous, current))
                    continue;

                if (!_projection.TryProject(previous.LocationName, previous.TilePosition, out Vector2 start))
                    continue;

                if (!_projection.TryProject(current.LocationName, current.TilePosition, out Vector2 end))
                    continue;

                float startAgeRatio = FootprintRenderPlanner.GetPointAgeRatio(i - 1, path.Count);
                float endAgeRatio = FootprintRenderPlanner.GetPointAgeRatio(i, path.Count);
                DrawSegment(spriteBatch, start, end, current.MovementType, startAgeRatio, endAgeRatio, transform);
            }
        }
    }

    private void DrawSegment(SpriteBatch spriteBatch, Vector2 mapStart, Vector2 mapEnd, MovementType movementType, float startAgeRatio, float endAgeRatio, MapScreenTransform transform)
    {
        DrawSegment(spriteBatch, transform.MapToScreen(mapStart), transform.MapToScreen(mapEnd), movementType, startAgeRatio, endAgeRatio, transform.Zoom);
    }

    private void DrawSegment(SpriteBatch spriteBatch, Vector2 mapStart, Vector2 mapEnd, MovementType movementType, float startAgeRatio, float endAgeRatio, MapContentTransform transform)
    {
        DrawSegment(spriteBatch, transform.MapToScreen(mapStart), transform.MapToScreen(mapEnd), movementType, startAgeRatio, endAgeRatio, transform.Zoom);
    }

    private void DrawSegment(SpriteBatch spriteBatch, Vector2 start, Vector2 end, MovementType movementType, float startAgeRatio, float endAgeRatio, float zoom)
    {
        Vector2 delta = end - start;
        float length = delta.Length();
        if (length < 6f)
            return;

        Vector2 direction = Vector2.Normalize(delta);
        float rotation = MathF.Atan2(direction.Y, direction.X) + MathHelper.PiOver2;
        float spacing = MathHelper.Clamp(22f * zoom, 12f, 36f);
        int steps = Math.Max(1, (int)(length / spacing));
        float scale = FootprintSpritePlanner.GetScale(zoom);

        for (int step = 1; step <= steps; step++)
        {
            float t = step / (float)steps;
            Vector2 position = Vector2.Lerp(start, end, t);
            float alpha = MathHelper.Lerp(0.25f, 1f, MathHelper.Lerp(startAgeRatio, endAgeRatio, t));
            Color color = movementType == MovementType.Suspicious ? new Color(45, 45, 45) * alpha : Color.Black * alpha;

            foreach (FootprintSprite sprite in FootprintSpritePlanner.PlanFootSprites(step - 1))
            {
                Vector2 rotatedOffset = Vector2.Transform(sprite.LocalOffset, Matrix.CreateRotationZ(rotation));
                Vector2 footPosition = position + rotatedOffset;
                spriteBatch.Draw(
                    _footprintTexture,
                    footPosition,
                    sprite.SourceRectangle,
                    color,
                    rotation,
                    sprite.Origin,
                    scale,
                    SpriteEffects.None,
                    0f);
            }
        }
    }

    private static Texture2D CreateFootprintTexture(GraphicsDevice graphicsDevice, string modDirectory)
    {
        string assetPath = GetFootprintAssetPath(modDirectory);
        if (File.Exists(assetPath))
        {
            try
            {
                using FileStream stream = File.OpenRead(assetPath);
                return Texture2D.FromStream(graphicsDevice, stream);
            }
            catch
            {
                return CreateFallbackFootprintTexture(graphicsDevice);
            }
        }

        return CreateFallbackFootprintTexture(graphicsDevice);
    }

    public static string GetFootprintAssetPath(string modDirectory)
    {
        return Path.Combine(modDirectory, FootprintAssetPath);
    }

    private static Texture2D CreateFallbackFootprintTexture(GraphicsDevice graphicsDevice)
    {
        FootprintMask mask = CreateFootprintMask();
        var texture = new Texture2D(graphicsDevice, mask.Width, mask.Height);
        texture.SetData(mask.Data);
        return texture;
    }

    public static FootprintMask CreateFootprintMask()
    {
        const int width = 18;
        const int height = 22;
        var data = new Color[width * height];

        for (int i = 0; i < data.Length; i++)
            data[i] = Color.Transparent;

        DrawFoot(data, width, xOffset: 2, yOffset: 1);
        DrawFoot(data, width, xOffset: 10, yOffset: 8);

        return new FootprintMask(width, height, data);
    }

    public void Dispose()
    {
        _footprintTexture?.Dispose();
        _footprintTexture = null;
    }

    private static void Set(Color[] data, int width, int x, int y)
    {
        data[y * width + x] = Color.White;
    }

    private static void DrawFoot(Color[] data, int width, int xOffset, int yOffset)
    {
        Set(data, width, xOffset + 2, yOffset + 0);
        Set(data, width, xOffset + 3, yOffset + 0);
        Set(data, width, xOffset + 1, yOffset + 1);
        Set(data, width, xOffset + 2, yOffset + 1);
        Set(data, width, xOffset + 3, yOffset + 1);
        Set(data, width, xOffset + 4, yOffset + 1);
        Set(data, width, xOffset + 1, yOffset + 2);
        Set(data, width, xOffset + 2, yOffset + 2);
        Set(data, width, xOffset + 3, yOffset + 2);
        Set(data, width, xOffset + 4, yOffset + 2);
        Set(data, width, xOffset + 2, yOffset + 3);
        Set(data, width, xOffset + 3, yOffset + 3);
        Set(data, width, xOffset + 2, yOffset + 5);
        Set(data, width, xOffset + 3, yOffset + 5);
        Set(data, width, xOffset + 1, yOffset + 6);
        Set(data, width, xOffset + 2, yOffset + 6);
        Set(data, width, xOffset + 3, yOffset + 6);
        Set(data, width, xOffset + 4, yOffset + 6);
        Set(data, width, xOffset + 1, yOffset + 7);
        Set(data, width, xOffset + 2, yOffset + 7);
        Set(data, width, xOffset + 3, yOffset + 7);
        Set(data, width, xOffset + 4, yOffset + 7);
        Set(data, width, xOffset + 2, yOffset + 8);
        Set(data, width, xOffset + 3, yOffset + 8);
    }
}

public readonly record struct FootprintMask(int Width, int Height, Color[] Data);
