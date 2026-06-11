namespace TheMarauderMap.Npc;

using TheMarauderMap.Localization;

public static class NpcDisplayNameService
{
    public static string GetMapDisplayName(string npcName, string displayName)
    {
        return GetMapDisplayName(npcName, displayName, isChinese: true);
    }

    public static string GetMapDisplayName(string npcName, string displayName, bool isChinese)
    {
        return MapLanguageService.GetNpcDisplayName(npcName, displayName, isChinese);
    }

    public static bool ShouldShowHeart(string npcName, string? spouseName)
    {
        return !string.IsNullOrWhiteSpace(spouseName)
            && string.Equals(npcName, spouseName, StringComparison.OrdinalIgnoreCase);
    }
}
