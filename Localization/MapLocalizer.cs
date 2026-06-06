namespace TheMarauderMap.Localization;

public enum MapLanguage
{
    English,
    Chinese
}

public static class MapLocalizer
{
    public static MapLanguage NormalizeLanguage(string languageCode)
    {
        return languageCode.StartsWith("zh", StringComparison.OrdinalIgnoreCase)
            ? MapLanguage.Chinese
            : MapLanguage.English;
    }

    public static string Text(string key, MapLanguage language)
    {
        return language == MapLanguage.Chinese && Chinese.TryGetValue(key, out string? chinese)
            ? chinese
            : English[key];
    }

    public static string GetNpcDisplayName(string npcName, string displayName, MapLanguage language)
    {
        return language == MapLanguage.Chinese ? displayName : npcName;
    }

    private static readonly Dictionary<string, string> English = new()
    {
        ["gmcm.open_map_key.name"] = "Open map key",
        ["gmcm.open_map_key.tooltip"] = "The keybind used to open or close The Marauder's Map.",
        ["gmcm.enable_footprints.name"] = "Enable footprints",
        ["gmcm.enable_footprints.tooltip"] = "Show recent fading NPC footprints on the map.",
        ["gmcm.enable_friendship_colors.name"] = "Enable friendship colors",
        ["gmcm.enable_friendship_colors.tooltip"] = "Color NPC names based on friendship heart level.",
        ["gmcm.footprint_interval.name"] = "Footprint interval",
        ["gmcm.footprint_interval.tooltip"] = "How often to record NPC positions in in-game minutes.",
        ["gmcm.visible_footprints.name"] = "Visible footprint points",
        ["gmcm.visible_footprints.tooltip"] = "How many recent footprint samples to draw per NPC."
    };

    private static readonly Dictionary<string, string> Chinese = new()
    {
        ["gmcm.open_map_key.name"] = "打开地图按键",
        ["gmcm.open_map_key.tooltip"] = "用于打开或关闭活点地图的按键。",
        ["gmcm.enable_footprints.name"] = "启用足迹",
        ["gmcm.enable_footprints.tooltip"] = "在地图上显示 NPC 最近逐渐淡出的足迹。",
        ["gmcm.enable_friendship_colors.name"] = "启用好感度颜色",
        ["gmcm.enable_friendship_colors.tooltip"] = "根据好感度心数给 NPC 名字上色。",
        ["gmcm.footprint_interval.name"] = "足迹记录间隔",
        ["gmcm.footprint_interval.tooltip"] = "每隔多少游戏内分钟记录一次 NPC 位置。",
        ["gmcm.visible_footprints.name"] = "可见足迹点数",
        ["gmcm.visible_footprints.tooltip"] = "每个 NPC 在地图上绘制多少个最近足迹采样点。"
    };
}
