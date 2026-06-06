using StardewModdingAPI;
using TheMarauderMap.Config;
using TheMarauderMap.Localization;

namespace TheMarauderMap.Integrations;

public static class GmcmConfigRegistrar
{
    private const string GenericModConfigMenuId = "spacechase0.GenericModConfigMenu";

    public static IReadOnlyList<string> GetOptionIds()
    {
        return new[]
        {
            nameof(MarauderMapConfig.OpenMapKey),
            nameof(MarauderMapConfig.EnableFootprints),
            nameof(MarauderMapConfig.EnableFriendshipColors),
            nameof(MarauderMapConfig.RecordIntervalMinutes),
            nameof(MarauderMapConfig.MaxVisibleFootprintPoints)
        };
    }

    public static void Register(IModHelper helper, IManifest manifest, Func<MarauderMapConfig> getConfig, Action<MarauderMapConfig> setConfig, Action save, Func<MapLanguage> getLanguage)
    {
        IGenericModConfigMenuApi? api = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>(GenericModConfigMenuId);
        if (api is null)
            return;

        api.Register(
            manifest,
            reset: () => setConfig(new MarauderMapConfig()),
            save: save
        );

        api.AddKeybindList(
            manifest,
            getValue: () => getConfig().OpenMapKey,
            setValue: value => getConfig().OpenMapKey = value,
            name: () => MapLocalizer.Text("gmcm.open_map_key.name", getLanguage()),
            tooltip: () => MapLocalizer.Text("gmcm.open_map_key.tooltip", getLanguage())
        );

        api.AddBoolOption(
            manifest,
            getValue: () => getConfig().EnableFootprints,
            setValue: value => getConfig().EnableFootprints = value,
            name: () => MapLocalizer.Text("gmcm.enable_footprints.name", getLanguage()),
            tooltip: () => MapLocalizer.Text("gmcm.enable_footprints.tooltip", getLanguage())
        );

        api.AddBoolOption(
            manifest,
            getValue: () => getConfig().EnableFriendshipColors,
            setValue: value => getConfig().EnableFriendshipColors = value,
            name: () => MapLocalizer.Text("gmcm.enable_friendship_colors.name", getLanguage()),
            tooltip: () => MapLocalizer.Text("gmcm.enable_friendship_colors.tooltip", getLanguage())
        );

        api.AddNumberOption(
            manifest,
            getValue: () => getConfig().RecordIntervalMinutes,
            setValue: value => getConfig().RecordIntervalMinutes = value,
            name: () => MapLocalizer.Text("gmcm.footprint_interval.name", getLanguage()),
            tooltip: () => MapLocalizer.Text("gmcm.footprint_interval.tooltip", getLanguage()),
            min: 10,
            max: 30,
            interval: 10
        );

        api.AddNumberOption(
            manifest,
            getValue: () => getConfig().MaxVisibleFootprintPoints,
            setValue: value => getConfig().MaxVisibleFootprintPoints = value,
            name: () => MapLocalizer.Text("gmcm.visible_footprints.name", getLanguage()),
            tooltip: () => MapLocalizer.Text("gmcm.visible_footprints.tooltip", getLanguage()),
            min: 2,
            max: 40,
            interval: 1
        );
    }
}
