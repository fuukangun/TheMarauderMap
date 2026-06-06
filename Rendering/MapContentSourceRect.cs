using Microsoft.Xna.Framework;
namespace TheMarauderMap.Rendering;

public static class MapContentSourceRect
{
    public static Rectangle ForViewport(Rectangle textureBounds, Vector2 panOffset, float zoom)
    {
        float visibleWidth = textureBounds.Width / zoom;
        float visibleHeight = textureBounds.Height / zoom;
        float centerX = textureBounds.X + textureBounds.Width / 2f + panOffset.X;
        float centerY = textureBounds.Y + textureBounds.Height / 2f + panOffset.Y;
        float x = MathHelper.Clamp(centerX - visibleWidth / 2f, textureBounds.X, textureBounds.Right - visibleWidth);
        float y = MathHelper.Clamp(centerY - visibleHeight / 2f, textureBounds.Y, textureBounds.Bottom - visibleHeight);

        return new Rectangle(
            (int)MathF.Round(x),
            (int)MathF.Round(y),
            Math.Max(1, (int)MathF.Round(visibleWidth)),
            Math.Max(1, (int)MathF.Round(visibleHeight)));
    }
}
