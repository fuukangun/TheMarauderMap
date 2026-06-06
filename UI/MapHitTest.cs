using Microsoft.Xna.Framework;

namespace TheMarauderMap.UI;

public static class MapHitTest
{
    public static bool IsWithinScreenRadius(Vector2 cursorScreenPosition, Vector2 targetScreenPosition, float radiusPixels)
    {
        return Vector2.Distance(cursorScreenPosition, targetScreenPosition) <= radiusPixels;
    }
}
