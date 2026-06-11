namespace TheMarauderMap.Localization;

public static class MapLanguageService
{
    public static bool IsChinese(string languageCode)
    {
        return languageCode.StartsWith("zh", StringComparison.OrdinalIgnoreCase);
    }

    public static string GetNpcDisplayName(string npcName, string displayName, bool isChinese)
    {
        return isChinese ? displayName : npcName;
    }
}
