using Microsoft.Xna.Framework;
using TheMarauderMap.Data;

namespace TheMarauderMap.Tracker;

public sealed class NpcFootprintTracker
{
    private const float NormalDistanceThreshold = 8f;
    private const float SuspiciousDistanceThreshold = 20f;

    private readonly Dictionary<string, NpcFootprintData> _footprints = new(StringComparer.OrdinalIgnoreCase);
    private readonly int _maxStoredPoints;

    public NpcFootprintTracker(int maxStoredPoints)
    {
        _maxStoredPoints = Math.Max(1, maxStoredPoints);
    }

    public void RecordPoint(string npcName, string locationName, Vector2 tilePosition, int timeOfDay)
    {
        if (string.IsNullOrWhiteSpace(npcName) || string.IsNullOrWhiteSpace(locationName))
            return;

        NpcFootprintData data = GetOrCreateData(npcName);
        FootprintPoint? previous = data.LastRecordedPoint;
        if (previous.HasValue
            && previous.Value.TimeOfDay == timeOfDay
            && !previous.Value.LocationName.Equals(locationName, StringComparison.OrdinalIgnoreCase))
            return;

        MovementType movementType = ClassifyMovement(previous, locationName, tilePosition);

        if (previous.HasValue
            && movementType == MovementType.Suspicious
            && previous.Value.LocationName.Equals(locationName, StringComparison.OrdinalIgnoreCase)
            && Vector2.Distance(previous.Value.TilePosition, tilePosition) > SuspiciousDistanceThreshold)
        {
            data.LastRecordedPoint = new FootprintPoint(locationName, tilePosition, timeOfDay, MovementType.Suspicious);
            data.BreakBeforeNextPoint = true;
            return;
        }

        if (data.BreakBeforeNextPoint)
        {
            movementType = MovementType.LocationJump;
            data.BreakBeforeNextPoint = false;
        }

        var point = new FootprintPoint(locationName, tilePosition, timeOfDay, movementType);
        data.LastRecordedPoint = point;

        if (movementType == MovementType.Normal && data.RecentPath.Count > 0)
        {
            FootprintPoint previousStored = data.RecentPath.Last();
            if (previousStored.LocationName.Equals(locationName, StringComparison.OrdinalIgnoreCase)
                && Vector2.Distance(previousStored.TilePosition, tilePosition) < 0.25f)
                return;
        }

        data.RecentPath.Enqueue(point);
        while (data.RecentPath.Count > _maxStoredPoints)
            data.RecentPath.Dequeue();
    }

    public IReadOnlyList<FootprintPoint> GetFootprints(string npcName)
    {
        if (_footprints.TryGetValue(npcName, out NpcFootprintData? data))
            return data.RecentPath.ToList();

        return Array.Empty<FootprintPoint>();
    }

    public IReadOnlyDictionary<string, IReadOnlyList<FootprintPoint>> GetAllFootprints()
    {
        return _footprints.ToDictionary(
            pair => pair.Key,
            pair => (IReadOnlyList<FootprintPoint>)pair.Value.RecentPath.ToList(),
            StringComparer.OrdinalIgnoreCase
        );
    }

    public int GetTrackedNpcCount()
    {
        return _footprints.Count;
    }

    public void ClearAll()
    {
        _footprints.Clear();
    }

    private NpcFootprintData GetOrCreateData(string npcName)
    {
        if (_footprints.TryGetValue(npcName, out NpcFootprintData? data))
            return data;

        data = new NpcFootprintData(npcName);
        _footprints[npcName] = data;
        return data;
    }

    private static MovementType ClassifyMovement(FootprintPoint? previous, string locationName, Vector2 tilePosition)
    {
        if (!previous.HasValue)
            return MovementType.Normal;

        FootprintPoint previousValue = previous.Value;
        if (!previousValue.LocationName.Equals(locationName, StringComparison.OrdinalIgnoreCase))
            return MovementType.LocationJump;

        float distance = Vector2.Distance(previousValue.TilePosition, tilePosition);
        if (distance <= NormalDistanceThreshold)
            return MovementType.Normal;

        return MovementType.Suspicious;
    }
}
