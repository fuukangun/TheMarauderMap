using Microsoft.Xna.Framework;
using TheMarauderMap.Rendering;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class MapContentSourceRectTests
{
    [Fact]
    public void ForViewport_AtTwoTimesZoom_UsesHalfSizedCenteredSource()
    {
        Rectangle source = MapContentSourceRect.ForViewport(
            new Rectangle(0, 0, 1200, 720),
            Vector2.Zero,
            2f);

        Assert.Equal(new Rectangle(300, 180, 600, 360), source);
    }
}
