using Microsoft.Xna.Framework;
using TheMarauderMap.Data;
using TheMarauderMap.Rendering;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class FootprintRenderPlannerTests
{
    [Fact]
    public void PlanPaths_WhenNpcIsNotSelected_UsesMostRecentTwoPoints()
    {
        var footprints = new Dictionary<string, IReadOnlyList<FootprintPoint>>
        {
            ["Maru"] = CreatePath(5)
        };

        IReadOnlyList<FootprintRenderPath> paths = FootprintRenderPlanner.PlanPaths(footprints, selectedNpcName: null);

        FootprintRenderPath path = Assert.Single(paths);
        Assert.Equal("Maru", path.NpcName);
        Assert.Equal(2, path.Points.Count);
        Assert.Equal(new Vector2(3, 0), path.Points[0].TilePosition);
        Assert.Equal(new Vector2(4, 0), path.Points[1].TilePosition);
    }

    [Fact]
    public void PlanPaths_WhenNpcIsSelected_UsesMostRecentTwelvePointsForSelectedNpcOnly()
    {
        var footprints = new Dictionary<string, IReadOnlyList<FootprintPoint>>
        {
            ["Maru"] = CreatePath(20),
            ["Haley"] = CreatePath(20)
        };

        IReadOnlyList<FootprintRenderPath> paths = FootprintRenderPlanner.PlanPaths(footprints, selectedNpcName: "Maru");

        FootprintRenderPath maru = Assert.Single(paths, path => path.NpcName == "Maru");
        FootprintRenderPath haley = Assert.Single(paths, path => path.NpcName == "Haley");
        Assert.Equal(12, maru.Points.Count);
        Assert.Equal(new Vector2(8, 0), maru.Points[0].TilePosition);
        Assert.Equal(new Vector2(19, 0), maru.Points[^1].TilePosition);
        Assert.Equal(2, haley.Points.Count);
    }

    [Fact]
    public void GetPointAgeRatio_MakesNewestPointDarkest()
    {
        float oldest = FootprintRenderPlanner.GetPointAgeRatio(pointIndex: 0, pointCount: 12);
        float newest = FootprintRenderPlanner.GetPointAgeRatio(pointIndex: 11, pointCount: 12);

        Assert.True(oldest < newest);
        Assert.Equal(1f, newest);
    }

    [Fact]
    public void ShouldConnectProjectedPoints_AllowsLocationJumpWhenBothMapPositionsAreKnown()
    {
        var previous = new FootprintPoint("Town", new Vector2(1, 1), 900, MovementType.Normal);
        var current = new FootprintPoint("BusStop", new Vector2(2, 2), 910, MovementType.LocationJump);

        bool shouldConnect = FootprintRenderPlanner.ShouldConnectProjectedPoints(previous, current);

        Assert.True(shouldConnect);
    }

    [Fact]
    public void ShouldConnectProjectedPoints_SkipsUnknownTeleportBreaks()
    {
        var previous = new FootprintPoint("Town", new Vector2(1, 1), 900, MovementType.Normal);
        var current = new FootprintPoint("Town", new Vector2(60, 60), 910, MovementType.LocationJump);

        bool shouldConnect = FootprintRenderPlanner.ShouldConnectProjectedPoints(previous, current);

        Assert.False(shouldConnect);
    }

    private static IReadOnlyList<FootprintPoint> CreatePath(int count)
    {
        return Enumerable.Range(0, count)
            .Select(index => new FootprintPoint("Town", new Vector2(index, 0), 900 + index, MovementType.Normal))
            .ToList();
    }
}
