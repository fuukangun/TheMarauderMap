using TheMarauderMap.Data;

namespace TheMarauderMap.Npc;

public static class NpcMapPositionResolver
{
    public static bool TryGetLatestTrackedPosition(
        string npcName,
        IReadOnlyDictionary<string, IReadOnlyList<FootprintPoint>> footprints,
        out FootprintPoint point)
    {
        if (footprints.TryGetValue(npcName, out IReadOnlyList<FootprintPoint>? path) && path.Count > 0)
        {
            point = path[^1];
            return true;
        }

        point = default;
        return false;
    }
}
