using Microsoft.Xna.Framework;
using TheMarauderMap.Data;

namespace TheMarauderMap.Scheduler;

public static class SchedulePredictionBuilder
{
    public static IReadOnlyList<PredictionPoint> BuildUpcomingPredictions(
        IEnumerable<SchedulePredictionEntry> entries,
        int currentTime,
        int maxPredictions)
    {
        if (maxPredictions <= 0)
            return Array.Empty<PredictionPoint>();

        return entries
            .Where(entry => entry.TimeOfDay > currentTime)
            .Where(entry => !string.IsNullOrWhiteSpace(entry.LocationName))
            .OrderBy(entry => entry.TimeOfDay)
            .Take(maxPredictions)
            .Select(entry => new PredictionPoint(entry.LocationName!, entry.TilePosition, entry.TimeOfDay, entry.Activity))
            .ToList();
    }
}

public readonly record struct SchedulePredictionEntry(
    int TimeOfDay,
    string? LocationName,
    Vector2 TilePosition,
    string? Activity
);
