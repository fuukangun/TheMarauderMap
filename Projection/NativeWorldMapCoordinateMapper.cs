using Microsoft.Xna.Framework;

namespace TheMarauderMap.Projection;

public static class NativeWorldMapCoordinateMapper
{
    public static Vector2 NormalizeToMarauderMap(Vector2 nativeMapPosition, Rectangle nativeMapBounds)
    {
        return nativeMapPosition;
    }

    public static Rectangle ScaleToMarauderMap(Rectangle nativeMapArea, Point nativeMapSize)
    {
        if (nativeMapSize.X <= 0 || nativeMapSize.Y <= 0)
            return Rectangle.Empty;

        float scaleX = (float)MapProjectionService.MapWidth / nativeMapSize.X;
        float scaleY = (float)MapProjectionService.MapHeight / nativeMapSize.Y;

        return new Rectangle(
            (int)(nativeMapArea.X * scaleX),
            (int)(nativeMapArea.Y * scaleY),
            (int)(nativeMapArea.Width * scaleX),
            (int)(nativeMapArea.Height * scaleY)
        );
    }
}
