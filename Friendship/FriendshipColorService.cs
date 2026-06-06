using Microsoft.Xna.Framework;

namespace TheMarauderMap.Friendship;

public static class FriendshipColorService
{
    public static Color GetColorForHeartLevel(int hearts)
    {
        int clamped = Math.Clamp(hearts, 0, 14);

        return clamped switch
        {
            <= 1 => new Color(255, 68, 68),
            <= 3 => new Color(255, 136, 68),
            <= 6 => new Color(255, 204, 68),
            <= 9 => new Color(136, 204, 68),
            <= 12 => new Color(68, 204, 136),
            _ => new Color(204, 136, 255)
        };
    }
}
