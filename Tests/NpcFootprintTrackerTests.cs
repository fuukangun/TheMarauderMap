using Microsoft.Xna.Framework;
using TheMarauderMap.Data;
using TheMarauderMap.Tracker;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class NpcFootprintTrackerTests
{
    [Fact]
    public void RecordPoint_FirstPoint_IsNormal()
    {
        var tracker = new NpcFootprintTracker(maxStoredPoints: 40);

        tracker.RecordPoint("Abigail", "Town", new Vector2(10, 10), 900);

        FootprintPoint point = Assert.Single(tracker.GetFootprints("Abigail"));
        Assert.Equal(MovementType.Normal, point.MovementType);
    }

    [Fact]
    public void RecordPoint_SameLocationSmallMove_IsNormal()
    {
        var tracker = new NpcFootprintTracker(maxStoredPoints: 40);

        tracker.RecordPoint("Abigail", "Town", new Vector2(10, 10), 900);
        tracker.RecordPoint("Abigail", "Town", new Vector2(14, 14), 910);

        FootprintPoint point = tracker.GetFootprints("Abigail").Last();
        Assert.Equal(MovementType.Normal, point.MovementType);
    }

    [Fact]
    public void RecordPoint_SameLocationMediumMove_IsSuspicious()
    {
        var tracker = new NpcFootprintTracker(maxStoredPoints: 40);

        tracker.RecordPoint("Abigail", "Town", new Vector2(10, 10), 900);
        tracker.RecordPoint("Abigail", "Town", new Vector2(22, 10), 910);

        FootprintPoint point = tracker.GetFootprints("Abigail").Last();
        Assert.Equal(MovementType.Suspicious, point.MovementType);
    }

    [Fact]
    public void RecordPoint_SameLocationHugeMove_DoesNotEnqueueTeleportPoint()
    {
        var tracker = new NpcFootprintTracker(maxStoredPoints: 40);

        tracker.RecordPoint("Abigail", "Town", new Vector2(10, 10), 900);
        tracker.RecordPoint("Abigail", "Town", new Vector2(60, 60), 910);

        Assert.Single(tracker.GetFootprints("Abigail"));
    }

    [Fact]
    public void RecordPoint_DifferentLocation_IsLocationJump()
    {
        var tracker = new NpcFootprintTracker(maxStoredPoints: 40);

        tracker.RecordPoint("Abigail", "Town", new Vector2(10, 10), 900);
        tracker.RecordPoint("Abigail", "Saloon", new Vector2(8, 14), 910);

        FootprintPoint point = tracker.GetFootprints("Abigail").Last();
        Assert.Equal(MovementType.LocationJump, point.MovementType);
    }

    [Fact]
    public void RecordPoint_SameNpcSameTime_IgnoresDuplicateNpcInstance()
    {
        var tracker = new NpcFootprintTracker(maxStoredPoints: 40);

        tracker.RecordPoint("Qi", "Club", new Vector2(10, 10), 900);
        tracker.RecordPoint("Qi", "QiNutRoom", new Vector2(20, 20), 900);

        FootprintPoint point = Assert.Single(tracker.GetFootprints("Qi"));
        Assert.Equal("Club", point.LocationName);
    }

    [Fact]
    public void RecordPoint_HugeMoveThenSmallMove_BreaksPathBeforePostTeleportPoint()
    {
        var tracker = new NpcFootprintTracker(maxStoredPoints: 40);

        tracker.RecordPoint("Abigail", "Town", new Vector2(10, 10), 900);
        tracker.RecordPoint("Abigail", "Town", new Vector2(60, 60), 910);
        tracker.RecordPoint("Abigail", "Town", new Vector2(61, 60), 920);

        IReadOnlyList<FootprintPoint> points = tracker.GetFootprints("Abigail");
        Assert.Equal(2, points.Count);
        Assert.Equal(new Vector2(61, 60), points[1].TilePosition);
        Assert.Equal(MovementType.LocationJump, points[1].MovementType);
    }

    [Fact]
    public void RecordPoint_ExceedsMaxStoredPoints_DropsOldest()
    {
        var tracker = new NpcFootprintTracker(maxStoredPoints: 3);

        tracker.RecordPoint("Abigail", "Town", new Vector2(1, 1), 900);
        tracker.RecordPoint("Abigail", "Town", new Vector2(2, 1), 910);
        tracker.RecordPoint("Abigail", "Town", new Vector2(3, 1), 920);
        tracker.RecordPoint("Abigail", "Town", new Vector2(4, 1), 930);

        IReadOnlyList<FootprintPoint> points = tracker.GetFootprints("Abigail");
        Assert.Equal(3, points.Count);
        Assert.Equal(new Vector2(2, 1), points[0].TilePosition);
    }

    [Fact]
    public void ClearAll_RemovesAllFootprints()
    {
        var tracker = new NpcFootprintTracker(maxStoredPoints: 40);
        tracker.RecordPoint("Abigail", "Town", new Vector2(10, 10), 900);

        tracker.ClearAll();

        Assert.Empty(tracker.GetFootprints("Abigail"));
    }
}
