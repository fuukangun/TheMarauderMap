using StardewValley;

namespace TheMarauderMap.Npc;

public static class NpcFilter
{
    public static bool ShouldShowOnMap(NPC npc)
    {
        return !npc.IsMonster
            && npc.currentLocation is not null
            && !string.IsNullOrWhiteSpace(npc.Name)
            && !string.IsNullOrWhiteSpace(npc.displayName);
    }
}
