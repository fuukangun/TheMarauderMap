using TheMarauderMap.Projection;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class MapDisplayContextTests
{
    [Theory]
    [InlineData("IslandSouth")]
    [InlineData("IslandNorth")]
    [InlineData("IslandWest")]
    [InlineData("IslandEast")]
    [InlineData("IslandFarmHouse")]
    public void AllowsLocation_WhenContextIsGingerIsland_OnlyAllowsIslandLocations(string locationName)
    {
        Assert.True(MapDisplayContext.GingerIsland.AllowsLocation(locationName));
        Assert.False(MapDisplayContext.Mainland.AllowsLocation(locationName));
    }

    [Theory]
    [InlineData("Town")]
    [InlineData("Saloon")]
    [InlineData("Farm")]
    [InlineData("Desert")]
    public void AllowsLocation_WhenContextIsMainland_RejectsIslandLocations(string locationName)
    {
        Assert.True(MapDisplayContext.Mainland.AllowsLocation(locationName));
        Assert.False(MapDisplayContext.GingerIsland.AllowsLocation(locationName));
    }

    [Theory]
    [InlineData("IslandSouth")]
    [InlineData("IslandFarmHouse")]
    public void FromPlayerLocation_UsesGingerIslandForIslandLocations(string locationName)
    {
        Assert.Equal(MapDisplayContext.GingerIsland, MapDisplayContext.FromPlayerLocation(locationName));
    }

    [Theory]
    [InlineData("Town")]
    [InlineData("Farm")]
    [InlineData(null)]
    public void FromPlayerLocation_UsesMainlandForOtherLocations(string? locationName)
    {
        Assert.Equal(MapDisplayContext.Mainland, MapDisplayContext.FromPlayerLocation(locationName));
    }
}
