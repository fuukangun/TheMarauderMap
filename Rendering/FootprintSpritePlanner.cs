using Microsoft.Xna.Framework;

namespace TheMarauderMap.Rendering;

public static class FootprintSpritePlanner
{
    private const int FrameWidth = 50;
    private const int FrameHeight = 110;
    private const int CompleteFrameIndex = 3;

    public static float GetScale(float zoom)
    {
        return MathHelper.Clamp(zoom * 0.198f, 0.18f, 0.378f);
    }

    public static IReadOnlyList<FootprintSprite> PlanFootSprites(int footstepIndex)
    {
        int sourceX = CompleteFrameIndex * FrameWidth;

        return new[]
        {
            new FootprintSprite(
                new Rectangle(sourceX, 0, FrameWidth, FrameHeight),
                new Vector2(-5f, 7f),
                new Vector2(FrameWidth / 2f, FrameHeight / 2f)),
            new FootprintSprite(
                new Rectangle(sourceX, FrameHeight, FrameWidth, FrameHeight),
                new Vector2(5f, -7f),
                new Vector2(FrameWidth / 2f, FrameHeight / 2f))
        };
    }
}

public sealed record FootprintSprite(Rectangle SourceRectangle, Vector2 LocalOffset, Vector2 Origin);
