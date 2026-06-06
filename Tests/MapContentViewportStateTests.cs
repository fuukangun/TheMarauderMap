using Microsoft.Xna.Framework;
using TheMarauderMap.UI;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class MapContentViewportStateTests
{
    [Fact]
    public void Constructor_StartsAtVanillaZoomWithNoPan()
    {
        var state = new MapContentViewportState(new Rectangle(100, 50, 800, 600));

        Assert.Equal(1f, state.Zoom);
        Assert.Equal(Vector2.Zero, state.PanOffset);
        Assert.False(state.IsZoomed);
    }

    [Fact]
    public void ZoomIn_CentersOnContentCenter()
    {
        var state = new MapContentViewportState(new Rectangle(100, 50, 800, 600));

        state.ZoomBy(+1);

        Assert.Equal(1.1f, state.Zoom, 2);
        Assert.Equal(Vector2.Zero, state.PanOffset);
    }

    [Fact]
    public void ZoomIn_ClampsAtThreeTimes()
    {
        var state = new MapContentViewportState(new Rectangle(100, 50, 800, 600));

        for (int i = 0; i < 40; i++)
            state.ZoomBy(+1);

        Assert.Equal(3f, state.Zoom, 2);
    }

    [Fact]
    public void Pan_IgnoredAtZoomOne()
    {
        var state = new MapContentViewportState(new Rectangle(100, 50, 800, 600));

        state.Pan(new Vector2(200, 150));

        Assert.Equal(Vector2.Zero, state.PanOffset);
    }

    [Fact]
    public void Pan_ClampsToContentEdgesWhenZoomed()
    {
        var state = new MapContentViewportState(
            new Rectangle(100, 50, 800, 600),
            new Rectangle(0, 0, 1600, 1000));
        for (int i = 0; i < 10; i++)
            state.ZoomBy(+1);

        state.Pan(new Vector2(9999, 9999));

        Assert.Equal(400f, state.PanOffset.X, 2);
        Assert.Equal(250f, state.PanOffset.Y, 2);
    }

    [Fact]
    public void ZoomOutToOne_ResetsPanOffset()
    {
        var state = new MapContentViewportState(new Rectangle(100, 50, 800, 600));
        state.ZoomBy(+1);

        state.ZoomBy(-1);

        Assert.Equal(1f, state.Zoom);
        Assert.Equal(Vector2.Zero, state.PanOffset);
    }

    [Fact]
    public void SetContentBounds_WhenBoundsChange_ResetsZoomAndPan()
    {
        var state = new MapContentViewportState(new Rectangle(100, 50, 800, 600));
        state.ZoomBy(+1);
        state.Pan(new Vector2(50, 50));

        state.SetContentBounds(new Rectangle(200, 100, 900, 650));

        Assert.Equal(1f, state.Zoom);
        Assert.Equal(Vector2.Zero, state.PanOffset);
    }

    [Fact]
    public void SetContentBounds_WhenOnlyScreenBoundsChange_PreservesZoomAndPan()
    {
        var state = new MapContentViewportState(
            new Rectangle(100, 50, 800, 600),
            new Rectangle(0, 0, 1600, 1000));
        state.ZoomBy(+1);
        state.Pan(new Vector2(50, 25));

        state.SetContentBounds(
            new Rectangle(120, 70, 800, 600),
            new Rectangle(0, 0, 1600, 1000));

        Assert.True(state.IsZoomed);
        Assert.Equal(50f, state.PanOffset.X, 2);
        Assert.Equal(25f, state.PanOffset.Y, 2);
    }

    [Fact]
    public void PanScreenDelta_ConvertsScreenMovementToContentPan()
    {
        var state = new MapContentViewportState(new Rectangle(100, 50, 800, 600));
        for (int i = 0; i < 10; i++)
            state.ZoomBy(+1);

        state.PanScreenDelta(new Vector2(-100, -60));

        Assert.Equal(50f, state.PanOffset.X, 2);
        Assert.Equal(30f, state.PanOffset.Y, 2);
    }

    [Fact]
    public void Pan_UsesContentBoundsCoordinateSpace()
    {
        var state = new MapContentViewportState(
            new Rectangle(100, 50, 800, 600),
            new Rectangle(0, 0, 1600, 1000));
        for (int i = 0; i < 10; i++)
            state.ZoomBy(+1);

        state.Pan(new Vector2(9999, 9999));

        Assert.Equal((1600f - 1600f / state.Zoom) / 2f, state.PanOffset.X, 2);
        Assert.Equal((1000f - 1000f / state.Zoom) / 2f, state.PanOffset.Y, 2);
    }
}
