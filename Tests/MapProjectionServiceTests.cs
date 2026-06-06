using Microsoft.Xna.Framework;
using TheMarauderMap.Projection;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class MapProjectionServiceTests
{
    [Fact]
    public void TryProject_OutdoorLocation_ReturnsPointInsideMapBounds()
    {
        var service = new MapProjectionService();

        bool success = service.TryProject("Town", new Vector2(50, 50), out Vector2 result);

        Assert.True(success);
        Assert.InRange(result.X, 0, MapProjectionService.MapWidth);
        Assert.InRange(result.Y, 0, MapProjectionService.MapHeight);
    }

    [Fact]
    public void TryProject_IndoorLocation_ReturnsAnchor()
    {
        var service = new MapProjectionService();

        bool success = service.TryProject("Saloon", new Vector2(10, 12), out Vector2 result);

        Assert.True(success);
        Assert.Equal(new Vector2(955, 560), result);
    }

    [Fact]
    public void TryProject_UnknownLocation_ReturnsFalse()
    {
        var service = new MapProjectionService();

        bool success = service.TryProject("UnknownModDungeon", new Vector2(10, 10), out Vector2 result);

        Assert.False(success);
        Assert.Equal(Vector2.Zero, result);
    }

    [Fact]
    public void TryGetLocationAnchor_KnownLocation_ReturnsAnchor()
    {
        var service = new MapProjectionService();

        bool success = service.TryGetLocationAnchor("ScienceHouse", out Vector2 result);

        Assert.True(success);
        Assert.Equal(new Vector2(770, 310), result);
    }
}
