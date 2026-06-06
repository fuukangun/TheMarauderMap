using Microsoft.Xna.Framework;
using TheMarauderMap.UI;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class MapHitTestTests
{
    [Fact]
    public void IsWithinScreenRadius_UsesScreenPixels()
    {
        bool inside = MapHitTest.IsWithinScreenRadius(
            new Vector2(100, 100),
            new Vector2(130, 140),
            radiusPixels: 50f);

        Assert.True(inside);
    }

    [Fact]
    public void IsWithinScreenRadius_RejectsDistantTargets()
    {
        bool inside = MapHitTest.IsWithinScreenRadius(
            new Vector2(100, 100),
            new Vector2(200, 200),
            radiusPixels: 50f);

        Assert.False(inside);
    }
}
