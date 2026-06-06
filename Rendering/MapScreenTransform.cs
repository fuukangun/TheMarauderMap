using Microsoft.Xna.Framework;

namespace TheMarauderMap.Rendering;

public readonly record struct MapScreenTransform(Vector2 ViewportOffset, float Zoom, Rectangle? ScreenBounds)
{
    public Vector2 MapToScreen(Vector2 mapPosition)
    {
        if (ScreenBounds.HasValue)
        {
            Rectangle bounds = ScreenBounds.Value;
            Vector2 boundedMapPosition = new(
                bounds.X + mapPosition.X,
                bounds.Y + mapPosition.Y
            );

            return boundedMapPosition * Zoom - ViewportOffset;
        }

        return mapPosition * Zoom - ViewportOffset;
    }

    public Vector2 ScreenToMap(Vector2 screenPosition)
    {
        if (ScreenBounds.HasValue)
        {
            Rectangle bounds = ScreenBounds.Value;
            Vector2 unscaledScreenPosition = (screenPosition + ViewportOffset) / Zoom;
            return new Vector2(unscaledScreenPosition.X - bounds.X, unscaledScreenPosition.Y - bounds.Y);
        }

        return (screenPosition + ViewportOffset) / Zoom;
    }
}
