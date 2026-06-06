using TheMarauderMap.Data;

namespace TheMarauderMap.Rendering;

public static class FootprintRenderPlanner
{
    public const int DefaultVisiblePoints = 2;
    public const int SelectedVisiblePoints = 12;

    public static IReadOnlyList<FootprintRenderPath> PlanPaths(
        IReadOnlyDictionary<string, IReadOnlyList<FootprintPoint>> footprints,
        string? selectedNpcName)
    {
        var paths = new List<FootprintRenderPath>();

        foreach ((string npcName, IReadOnlyList<FootprintPoint> path) in footprints)
        {
            int maxVisiblePoints = string.Equals(npcName, selectedNpcName, StringComparison.OrdinalIgnoreCase)
                ? SelectedVisiblePoints
                : DefaultVisiblePoints;
            int visibleCount = Math.Min(path.Count, maxVisiblePoints);
            if (visibleCount < 2)
                continue;

            int startIndex = path.Count - visibleCount;
            paths.Add(new FootprintRenderPath(npcName, path.Skip(startIndex).ToList()));
        }

        return paths;
    }

    public static float GetPointAgeRatio(int pointIndex, int pointCount)
    {
        if (pointCount <= 1)
            return 1f;

        return pointIndex / (float)(pointCount - 1);
    }

    public static bool ShouldConnectProjectedPoints(FootprintPoint previous, FootprintPoint current)
    {
        if (previous.LocationName.Equals(current.LocationName, StringComparison.OrdinalIgnoreCase))
            return current.MovementType != MovementType.LocationJump;

        return current.MovementType == MovementType.LocationJump;
    }
}

public sealed record FootprintRenderPath(string NpcName, IReadOnlyList<FootprintPoint> Points);
