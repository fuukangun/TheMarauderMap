using Microsoft.Xna.Framework;
using TheMarauderMap.Scheduler;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class SchedulePredictionBuilderTests
{
    [Fact]
    public void BuildUpcomingPredictions_SortsFutureScheduleEntriesByTime()
    {
        var entries = new[]
        {
            new SchedulePredictionEntry(1400, "Town", new Vector2(10, 10), null),
            new SchedulePredictionEntry(1000, "Saloon", new Vector2(4, 5), null),
            new SchedulePredictionEntry(1200, "Mountain", new Vector2(8, 9), null)
        };

        var predictions = SchedulePredictionBuilder.BuildUpcomingPredictions(entries, currentTime: 900, maxPredictions: 4);

        Assert.Collection(
            predictions,
            point => Assert.Equal(1000, point.TimeOfDay),
            point => Assert.Equal(1200, point.TimeOfDay),
            point => Assert.Equal(1400, point.TimeOfDay)
        );
    }

    [Fact]
    public void BuildUpcomingPredictions_IgnoresPastEntriesAndEntriesWithoutLocations()
    {
        var entries = new[]
        {
            new SchedulePredictionEntry(800, "Town", new Vector2(10, 10), null),
            new SchedulePredictionEntry(1000, "", new Vector2(4, 5), null),
            new SchedulePredictionEntry(1100, null, new Vector2(4, 5), null),
            new SchedulePredictionEntry(1200, "Mountain", new Vector2(8, 9), null)
        };

        var predictions = SchedulePredictionBuilder.BuildUpcomingPredictions(entries, currentTime: 900, maxPredictions: 4);

        var point = Assert.Single(predictions);
        Assert.Equal(1200, point.TimeOfDay);
        Assert.Equal("Mountain", point.LocationName);
    }
}
