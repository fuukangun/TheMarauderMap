using Microsoft.Xna.Framework;
using TheMarauderMap.Projection;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class NativeWorldMapCoordinateMapperTests
{
    [Fact]
    public void NormalizeToMarauderMap_KeepsNativeMapLocalPixels()
    {
        Vector2 result = NativeWorldMapCoordinateMapper.NormalizeToMarauderMap(
            new Vector2(400, 300),
            new Rectangle(500, 200, 800, 600));

        Assert.Equal(new Vector2(400, 300), result);
    }

    [Fact]
    public void ScaleToMarauderMap_UsesMapAreaAsMapLocalCoordinates()
    {
        Rectangle result = NativeWorldMapCoordinateMapper.ScaleToMarauderMap(
            new Rectangle(400, 300, 100, 60),
            new Point(800, 600));

        Assert.Equal(new Rectangle(800, 500, 200, 100), result);
    }
}
