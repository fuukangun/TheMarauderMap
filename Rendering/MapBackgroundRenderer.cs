using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.WorldMaps;
using TheMarauderMap.Projection;

namespace TheMarauderMap.Rendering;

public sealed class MapBackgroundRenderer : IDisposable
{
    public const MapBackgroundMode DefaultMode = MapBackgroundMode.VanillaMapPage;

    private Texture2D? _pixel;
    private Texture2D? _vanillaMapTexture;
    private bool _textureLoadAttempted;
    private MapPage? _vanillaMapPage;
    private Point _vanillaMapPageSize;

    public Rectangle? VanillaMapBounds { get; private set; }
    public Rectangle? ContentBounds { get; private set; }
    public Rectangle? MapCoordinateBounds { get; private set; }
    public MapContentInsets ContentInsets { get; set; } = new(0, 0, 0, 0);
    public bool IsUsingVanillaMapPage => VanillaMapBounds.HasValue;

    public static Rectangle GetContentBounds(Rectangle mapBounds, MapContentInsets insets)
    {
        int x = mapBounds.X + insets.Left;
        int y = mapBounds.Y + insets.Top;
        int width = Math.Max(1, mapBounds.Width - insets.Left - insets.Right);
        int height = Math.Max(1, mapBounds.Height - insets.Top - insets.Bottom);
        return new Rectangle(x, y, width, height);
    }

    public static Rectangle ScaleMapBoundsToUiPixels(Rectangle mapBounds, int pixelZoom)
    {
        int scale = Math.Max(1, pixelZoom);
        return new Rectangle(
            mapBounds.X,
            mapBounds.Y,
            mapBounds.Width * scale,
            mapBounds.Height * scale);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 viewportOffset, float zoom)
    {
        if (DrawVanillaMapPage(spriteBatch))
            return;

        if (!DrawWorldMap(spriteBatch, viewportOffset, zoom))
        {
            TryLoadVanillaMapTexture();
            if (_vanillaMapTexture is not null)
                DrawVanillaMap(spriteBatch, viewportOffset, zoom);
            else
                DrawDebugBackground(spriteBatch, viewportOffset, zoom);
        }
    }

    private bool DrawVanillaMapPage(SpriteBatch spriteBatch)
    {
        try
        {
            Point viewportSize = new(Game1.uiViewport.Width, Game1.uiViewport.Height);

            if (_vanillaMapPage is null || _vanillaMapPageSize != viewportSize)
            {
                _vanillaMapPage = new MapPage(0, 0, viewportSize.X, viewportSize.Y);
                _vanillaMapPageSize = viewportSize;
            }

            _vanillaMapPage.draw(spriteBatch);
            VanillaMapBounds = _vanillaMapPage.mapBounds.Width > 0 && _vanillaMapPage.mapBounds.Height > 0
                ? ScaleMapBoundsToUiPixels(_vanillaMapPage.mapBounds, Game1.pixelZoom)
                : new Rectangle(0, 0, viewportSize.X, viewportSize.Y);
            ContentBounds = GetContentBounds(VanillaMapBounds.Value, ContentInsets);
            MapCoordinateBounds = GetMapCoordinateBounds() ?? new Rectangle(0, 0, ContentBounds.Value.Width, ContentBounds.Value.Height);
            return true;
        }
        catch
        {
            VanillaMapBounds = null;
            ContentBounds = null;
            MapCoordinateBounds = null;
            return false;
        }
    }

    private static Rectangle? GetMapCoordinateBounds()
    {
        try
        {
            Rectangle? bounds = null;
            foreach (MapRegion region in WorldMapManager.GetMapRegions())
            {
                Rectangle regionBounds = region.GetMapPixelBounds();
                if (regionBounds.Width <= 0 || regionBounds.Height <= 0)
                    continue;

                bounds = bounds.HasValue
                    ? Rectangle.Union(bounds.Value, regionBounds)
                    : regionBounds;
            }

            return bounds;
        }
        catch
        {
            return null;
        }
    }

    private static bool DrawWorldMap(SpriteBatch spriteBatch, Vector2 viewportOffset, float zoom)
    {
        try
        {
            bool drewAnyTexture = false;

            foreach (MapRegion region in WorldMapManager.GetMapRegions())
            {
                Rectangle regionBounds = region.GetMapPixelBounds();

                MapAreaTexture? baseTexture = region.GetBaseTexture();
                if (baseTexture is not null)
                {
                    DrawWorldMapTexture(spriteBatch, baseTexture, regionBounds, viewportOffset, zoom);
                    drewAnyTexture = true;
                }

                foreach (MapArea area in region.GetAreas())
                {
                    foreach (MapAreaTexture texture in area.GetTextures())
                    {
                        DrawWorldMapTexture(spriteBatch, texture, regionBounds, viewportOffset, zoom);
                        drewAnyTexture = true;
                    }
                }
            }

            return drewAnyTexture;
        }
        catch
        {
            return false;
        }
    }

    private static void DrawWorldMapTexture(SpriteBatch spriteBatch, MapAreaTexture texture, Rectangle regionBounds, Vector2 viewportOffset, float zoom)
    {
        Rectangle mapArea = texture.GetOffsetMapPixelArea(0, 0);
        Rectangle destination = NativeWorldMapCoordinateMapper.ScaleToMarauderMap(mapArea, regionBounds.Size);
        Rectangle screenDestination = MapDrawTransform.ToScreenRectangle(destination, viewportOffset, zoom);

        spriteBatch.Draw(texture.Texture, screenDestination, texture.SourceRect, Color.White);
    }

    private void TryLoadVanillaMapTexture()
    {
        if (_textureLoadAttempted)
            return;

        _textureLoadAttempted = true;

        foreach (string assetName in MapTextureAssetSelector.GetLegacyFallbackAssetNames())
        {
            try
            {
                _vanillaMapTexture = Game1.content.Load<Texture2D>(assetName);
                return;
            }
            catch
            {
            }
        }

        _vanillaMapTexture = null;
    }

    private void DrawVanillaMap(SpriteBatch spriteBatch, Vector2 viewportOffset, float zoom)
    {
        float scaleX = (float)MapProjectionService.MapWidth / _vanillaMapTexture!.Width * zoom;
        float scaleY = (float)MapProjectionService.MapHeight / _vanillaMapTexture.Height * zoom;
        Vector2 position = new(-viewportOffset.X, -viewportOffset.Y);
        spriteBatch.Draw(_vanillaMapTexture, position, null, Color.White, 0f, Vector2.Zero, new Vector2(scaleX, scaleY), SpriteEffects.None, 0f);
    }

    private void DrawDebugBackground(SpriteBatch spriteBatch, Vector2 viewportOffset, float zoom)
    {
        _pixel ??= CreatePixelTexture(Game1.graphics.GraphicsDevice);

        Rectangle mapRect = MapDrawTransform.ToScreenRectangle(new Rectangle(0, 0, MapProjectionService.MapWidth, MapProjectionService.MapHeight), viewportOffset, zoom);
        spriteBatch.Draw(_pixel, mapRect, new Color(41, 30, 20));

        DrawRegion(spriteBatch, new Rectangle(70, 360, 390, 360), viewportOffset, zoom, new Color(78, 58, 34), "Farm");
        DrawRegion(spriteBatch, new Rectangle(650, 390, 500, 360), viewportOffset, zoom, new Color(68, 48, 32), "Town");
        DrawRegion(spriteBatch, new Rectangle(680, 130, 460, 240), viewportOffset, zoom, new Color(58, 70, 44), "Mountain");
        DrawRegion(spriteBatch, new Rectangle(470, 720, 420, 220), viewportOffset, zoom, new Color(45, 65, 38), "Forest");
        DrawRegion(spriteBatch, new Rectangle(900, 760, 360, 180), viewportOffset, zoom, new Color(82, 68, 42), "Beach");
    }

    private void DrawRegion(SpriteBatch spriteBatch, Rectangle mapRect, Vector2 viewportOffset, float zoom, Color color, string label)
    {
        _pixel ??= CreatePixelTexture(Game1.graphics.GraphicsDevice);
        Rectangle screen = MapDrawTransform.ToScreenRectangle(mapRect, viewportOffset, zoom);
        spriteBatch.Draw(_pixel, screen, color * 0.85f);
        spriteBatch.DrawString(Game1.smallFont, label, new Vector2(screen.X + 8, screen.Y + 8), Color.Wheat);
    }

    public void Dispose()
    {
        _pixel?.Dispose();
        _pixel = null;
    }

    private static Texture2D CreatePixelTexture(GraphicsDevice graphicsDevice)
    {
        var texture = new Texture2D(graphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        return texture;
    }

}
