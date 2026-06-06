using Microsoft.Xna.Framework;
using TheMarauderMap.Rendering;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class MapBackgroundRendererTests
{
    [Fact]
    public void ToScreenRectangle_AppliesViewportAndZoomConsistently()
    {
        Rectangle actual = MapDrawTransform.ToScreenRectangle(
            new Rectangle(20, 30, 100, 50),
            new Vector2(5, 10),
            2f);

        Assert.Equal(new Rectangle(35, 50, 200, 100), actual);
    }

    [Fact]
    public void MapToScreen_WithVanillaBounds_AddsNativeMapPositionToBounds()
    {
        var transform = new MapScreenTransform(Vector2.Zero, 1f, new Rectangle(100, 50, 800, 600));

        Vector2 actual = transform.MapToScreen(new Vector2(400, 300));

        Assert.Equal(new Vector2(500, 350), actual);
    }

    [Fact]
    public void GetLegacyFallbackAssetNames_UsesVanillaLooseSpritesMapAtlas()
    {
        IReadOnlyList<string> assetNames = MapTextureAssetSelector.GetLegacyFallbackAssetNames();

        Assert.Contains("LooseSprites/map", assetNames);
    }

    [Fact]
    public void DefaultBackgroundMode_UsesVanillaMapPage()
    {
        Assert.Equal(MapBackgroundMode.VanillaMapPage, MapBackgroundRenderer.DefaultMode);
    }

    [Fact]
    public void GetContentBounds_UsesConfiguredInsetsInsideMapBounds()
    {
        Rectangle result = MapBackgroundRenderer.GetContentBounds(
            new Rectangle(100, 50, 800, 600),
            new MapContentInsets(Left: 10, Top: 20, Right: 30, Bottom: 40));

        Assert.Equal(new Rectangle(110, 70, 760, 540), result);
    }

    [Fact]
    public void GetContentBounds_ClampsInvalidInsetSizeToOnePixel()
    {
        Rectangle result = MapBackgroundRenderer.GetContentBounds(
            new Rectangle(100, 50, 20, 20),
            new MapContentInsets(Left: 15, Top: 15, Right: 15, Bottom: 15));

        Assert.Equal(new Rectangle(115, 65, 1, 1), result);
    }

    [Fact]
    public void ScaleMapBoundsToUiPixels_ExpandsMapPageLogicalBoundsByPixelZoom()
    {
        Rectangle result = MapBackgroundRenderer.ScaleMapBoundsToUiPixels(
            new Rectangle(40, 0, 300, 180),
            4);

        Assert.Equal(new Rectangle(40, 0, 1200, 720), result);
    }
}
