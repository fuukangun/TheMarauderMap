namespace TheMarauderMap.Npc;

using TheMarauderMap.Localization;

public static class NpcDisplayNameService
{
    public static string GetMapDisplayName(string npcName, string displayName)
    {
        return GetMapDisplayName(npcName, displayName, MapLanguage.Chinese);
    }

    public static string GetMapDisplayName(string npcName, string displayName, MapLanguage language)
    {
        return MapLocalizer.GetNpcDisplayName(npcName, displayName, language);
    }

    public static bool ShouldShowHeart(string npcName, string? spouseName)
    {
        return !string.IsNullOrWhiteSpace(spouseName)
            && string.Equals(npcName, spouseName, StringComparison.OrdinalIgnoreCase);
    }
}
