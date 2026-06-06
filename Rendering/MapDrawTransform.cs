using Microsoft.Xna.Framework;

namespace TheMarauderMap.Rendering;

public static class MapDrawTransform
{
    public static Rectangle ToScreenRectangle(Rectangle mapRect, Vector2 viewportOffset, float zoom)
    {
        return new Rectangle(
            (int)(mapRect.X * zoom - viewportOffset.X),
            (int)(mapRect.Y * zoom - viewportOffset.Y),
            (int)(mapRect.Width * zoom),
            (int)(mapRect.Height * zoom)
        );
    }
}
