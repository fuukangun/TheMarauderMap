using TheMarauderMap.UI;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class MapOverlayStatsTests
{
    [Fact]
    public void GetTrackedNpcCount_ReflectsLatestTrackerState()
    {
        int trackedNpcCount = 2;
        var stats = new MapOverlayStats(() => trackedNpcCount, () => 900);

        trackedNpcCount = 5;

        Assert.Equal(5, stats.GetTrackedNpcCount());
    }
}
