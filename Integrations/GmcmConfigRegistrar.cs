using StardewModdingAPI;
using TheMarauderMap.Config;

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

    public static void Register(IModHelper helper, IManifest manifest, Func<MarauderMapConfig> getConfig, Action<MarauderMapConfig> setConfig, Action save)
    {
        IGenericModConfigMenuApi? api = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>(GenericModConfigMenuId);
        if (api is null)
            return;

        ITranslationHelper translations = helper.Translation;

        api.Register(
            manifest,
            reset: () => setConfig(new MarauderMapConfig()),
            save: save
        );

        api.AddKeybindList(
            manifest,
            getValue: () => getConfig().OpenMapKey,
            setValue: value => getConfig().OpenMapKey = value,
            name: () => translations.Get("gmcm.open_map_key.name"),
            tooltip: () => translations.Get("gmcm.open_map_key.tooltip")
        );

        api.AddBoolOption(
            manifest,
            getValue: () => getConfig().EnableFootprints,
            setValue: value => getConfig().EnableFootprints = value,
            name: () => translations.Get("gmcm.enable_footprints.name"),
            tooltip: () => translations.Get("gmcm.enable_footprints.tooltip")
        );

        api.AddBoolOption(
            manifest,
            getValue: () => getConfig().EnableFriendshipColors,
            setValue: value => getConfig().EnableFriendshipColors = value,
            name: () => translations.Get("gmcm.enable_friendship_colors.name"),
            tooltip: () => translations.Get("gmcm.enable_friendship_colors.tooltip")
        );

        api.AddNumberOption(
            manifest,
            getValue: () => getConfig().RecordIntervalMinutes,
            setValue: value => getConfig().RecordIntervalMinutes = value,
            name: () => translations.Get("gmcm.footprint_interval.name"),
            tooltip: () => translations.Get("gmcm.footprint_interval.tooltip"),
            min: 10,
            max: 30,
            interval: 10
        );

        api.AddNumberOption(
            manifest,
            getValue: () => getConfig().MaxVisibleFootprintPoints,
            setValue: value => getConfig().MaxVisibleFootprintPoints = value,
            name: () => translations.Get("gmcm.visible_footprints.name"),
            tooltip: () => translations.Get("gmcm.visible_footprints.tooltip"),
            min: 2,
            max: 40,
            interval: 1
        );
    }
}
