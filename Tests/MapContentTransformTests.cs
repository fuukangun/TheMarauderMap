using Microsoft.Xna.Framework;
using TheMarauderMap.Rendering;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class MapContentTransformTests
{
    [Fact]
    public void MapToScreen_AtZoomOne_MatchesStableVanillaBoundsOffset()
    {
        var transform = new MapContentTransform(new Rectangle(100, 50, 800, 600), Vector2.Zero, 1f);

        Vector2 actual = transform.MapToScreen(new Vector2(400, 300));

        Assert.Equal(new Vector2(500, 350), actual);
    }

    [Fact]
    public void MapToScreen_WhenZoomed_AppliesSamePanAndZoom()
    {
        var transform = new MapContentTransform(
            new Rectangle(100, 50, 800, 600),
            new Rectangle(0, 0, 1600, 1000),
            new Vector2(25, 15),
            2f);

        Vector2 actual = transform.MapToScreen(new Vector2(825, 515));

        Assert.Equal(new Vector2(900, 550), actual);
    }

    [Fact]
    public void MapToScreen_WhenZoomedWithNoPan_KeepsContentCenterFixed()
    {
        var transform = new MapContentTransform(
            new Rectangle(100, 50, 800, 600),
            new Rectangle(0, 0, 1600, 1000),
            Vector2.Zero,
            2f);

        Vector2 actual = transform.MapToScreen(new Vector2(800, 500));

        Assert.Equal(new Vector2(900, 550), actual);
    }

    [Fact]
    public void MapToScreen_WhenZoomed_DoesNotCompressNpcPositionsIntoUpperLeftQuadrant()
    {
        var transform = new MapContentTransform(
            new Rectangle(100, 50, 800, 600),
            new Rectangle(0, 0, 1600, 1000),
            Vector2.Zero,
            2f);

        Vector2 actual = transform.MapToScreen(new Vector2(1200, 800));

        Assert.True(actual.X > 900);
        Assert.True(actual.Y > 550);
    }

    [Fact]
    public void MapToScreen_WhenZoomed_UsesProvidedMapCoordinateCenter()
    {
        var transform = new MapContentTransform(
            new Rectangle(100, 50, 800, 600),
            new Rectangle(100, 50, 1200, 800),
            Vector2.Zero,
            2f);

        Vector2 actual = transform.MapToScreen(new Vector2(700, 450));

        Assert.Equal(new Vector2(700, 450), actual);
    }

    [Fact]
    public void MapToScreen_RectangleUsesStableMapLocalCoordinates()
    {
        var transform = new MapContentTransform(new Rectangle(100, 50, 800, 600), Vector2.Zero, 1f);

        Rectangle actual = transform.MapToScreen(new Rectangle(0, 0, 800, 600));

        Assert.Equal(new Rectangle(100, 50, 800, 600), actual);
    }

    [Fact]
    public void ScreenToMap_ReversesMapToScreen()
    {
        var transform = new MapContentTransform(new Rectangle(100, 50, 800, 600), new Vector2(200, 150), 2f);

        Vector2 map = transform.ScreenToMap(transform.MapToScreen(new Vector2(425, 275)));

        Assert.Equal(new Vector2(425, 275), map);
    }
}
