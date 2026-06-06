using Microsoft.Xna.Framework;

namespace TheMarauderMap.Rendering;

public readonly record struct MapContentTransform(Rectangle ContentBounds, Rectangle MapCoordinateBounds, Vector2 PanOffset, float Zoom)
{
    public MapContentTransform(Rectangle contentBounds, Vector2 panOffset, float zoom)
        : this(contentBounds, new Rectangle(0, 0, contentBounds.Width, contentBounds.Height), panOffset, zoom)
    {
    }

    public Vector2 MapToScreen(Vector2 mapLocal)
    {
        Vector2 mapCenter = MapCenter;
        Vector2 screenCenter = new Vector2(ContentBounds.X, ContentBounds.Y) + (mapCenter - new Vector2(MapCoordinateBounds.X, MapCoordinateBounds.Y));
        return screenCenter + (mapLocal - mapCenter - PanOffset) * Zoom;
    }

    public Vector2 ScreenToMap(Vector2 screen)
    {
        Vector2 mapCenter = MapCenter;
        Vector2 screenCenter = new Vector2(ContentBounds.X, ContentBounds.Y) + (mapCenter - new Vector2(MapCoordinateBounds.X, MapCoordinateBounds.Y));
        return (screen - screenCenter) / Zoom + mapCenter + PanOffset;
    }

    public Rectangle MapToScreen(Rectangle mapLocal)
    {
        Vector2 topLeft = MapToScreen(new Vector2(mapLocal.X, mapLocal.Y));
        return new Rectangle(
            (int)topLeft.X,
            (int)topLeft.Y,
            (int)(mapLocal.Width * Zoom),
            (int)(mapLocal.Height * Zoom));
    }

    private Vector2 MapCenter => new(
        MapCoordinateBounds.X + MapCoordinateBounds.Width / 2f,
        MapCoordinateBounds.Y + MapCoordinateBounds.Height / 2f);
}
